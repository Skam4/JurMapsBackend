using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;
using Microsoft.IdentityModel.Tokens;
using JurMaps.Model.Configuration;
using Microsoft.Extensions.Options;
using JurMaps.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RTools_NTS.Util;
using JurMaps.Controllers;
using static Google.Apis.Storage.v1.ChannelsResource;
using System.Security.Claims;
using JurMaps.Shared;


namespace JurMaps.Services
{
    public class UserService : IUserService
    {
        private readonly IMapService _mapService;
        private readonly SmtpSettings _smtpSettings;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserMapLikeRepository _userMapLikeRepository;
        private readonly IConfiguration _configuration;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ITokenService _tokenService;
        private readonly Helpers _helpers;

        public UserService(
            IMapService mapService,
            IOptions<SmtpSettings> smtpSettings,
            IFirebaseStorageService firebaseStorageService,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserMapLikeRepository userMapLikeRepository,
            IConfiguration configuration,
            IRedisCacheService redisCacheService,
            ITokenService tokenService,
            Helpers helpers
            )
        {
            _mapService = mapService;
            _smtpSettings = smtpSettings.Value;
            _firebaseStorageService = firebaseStorageService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userMapLikeRepository = userMapLikeRepository;
            _configuration = configuration;
            _redisCacheService = redisCacheService;
            _tokenService = tokenService;
            _helpers = helpers;
        }

        /// <summary>
        /// Rejestruje nowego użytkownika w systemie.
        /// </summary>
        /// <param name="model">Obiekt RegisterDto zawierający dane rejestracyjne</param>
        /// <exception cref="ArgumentException">Jeśli dane wejściowe są niepoprawne</exception>
        /// <exception cref="InvalidOperationException">Jeśli konto z podanym e-mailem lub nazwą użytkownika już istnieje</exception>
        public async Task RegisterUserAsync(RegisterDto model)
        {
            if (model == null)
                throw new ArgumentException("Nieprawidłowe dane.");

            float toxicityScore = await _helpers.AnalyzeText(model.Name);

            if (toxicityScore >= 0.10)
            {
                throw new InvalidOperationException("Nazwa użytkownika nie może być obraźliwa.");
            }

            string cacheKey = $"UserCache:{model.Email}";

            // Sprawdzamy cache, zanim pójdziemy do bazy
            User? existingUser = await _redisCacheService.GetValueAsync<User>(cacheKey);

            if (existingUser == null)
            {
                existingUser = await _userRepository.GetUserByEmailAsync(model.Email)
                               ?? await _userRepository.GetUserByUserNameAsync(model.Name);
            }

            if (existingUser != null)
                throw new InvalidOperationException(existingUser.UserEmail == model.Email
                    ? "Konto z tym adresem e-mail już istnieje."
                    : "Konto o takiej nazwie już istnieje.");

            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password, 13);

            var newUser = new User
            {
                UserName = model.Name,
                UserEmail = model.Email,
                UserPassword = hashedPassword,
                UserCreatedDate = DateTime.UtcNow.ToString("dd.MM.yyyy"),
                UserRole = await _roleRepository.GetRoleByRoleIdAsync(model.Role ?? 1)
            };

            newUser = await SendEmailVerificationAsync(newUser);
            await _userRepository.AddAsync(newUser);

            // Cache nowego użytkownika na 5 minut
            await _redisCacheService.SetValueAsync(cacheKey, newUser, TimeSpan.FromMinutes(5));
        }



        /// <summary>
        /// Loguje użytkownika na podstawie adresu e-mail i hasła.
        /// </summary>
        /// <param name="model">Obiekt LoginDto zawierający dane logowania</param>
        /// <exception cref="ArgumentException">Jeśli dane wejściowe są niepoprawne</exception>
        /// <exception cref="InvalidOperationException">Jeśli e-mail lub hasło są niepoprawne</exception>
        public async Task<object> LoginUserAsync(LoginDto model, string? ipAddress)
        {
            if (model == null)
                throw new ArgumentException("Niepoprawne dane");

            string email = model.Email.ToLower();
            string emailAttemptsKey = $"LoginAttempts:{email}";
            string ipAttemptsKey = $"LoginAttempts:{ipAddress}";
            string blockedKey = $"BlockedUser:{email}:{ipAddress}";

            int maxAttempts = int.Parse(_configuration["LoginSettings:MaxAttempts"]);
            TimeSpan blockDuration = TimeSpan.FromMinutes(int.Parse(_configuration["LoginSettings:BlockDurationInMinutes"]));

            if (await _redisCacheService.GetValueAsync<string>(blockedKey) != null)
                throw new InvalidOperationException("Przekroczono limit prób logowania. Spróbuj ponownie za minutę.");

            User? user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(model.Password, user.UserPassword))
            {
                int emailAttempts = int.TryParse(await _redisCacheService.GetValueAsync<string>(emailAttemptsKey), out var ea) ? ea + 1 : 1;
                int ipAttempts = int.TryParse(await _redisCacheService.GetValueAsync<string>(ipAttemptsKey), out var ia) ? ia + 1 : 1;

                await _redisCacheService.SetValueAsync(emailAttemptsKey, emailAttempts.ToString(), TimeSpan.FromMinutes(1));
                await _redisCacheService.SetValueAsync(ipAttemptsKey, ipAttempts.ToString(), TimeSpan.FromMinutes(1));

                if (emailAttempts >= maxAttempts || ipAttempts >= maxAttempts)
                {
                    await _redisCacheService.SetValueAsync(blockedKey, "true", blockDuration);
                    throw new InvalidOperationException("Przekroczono limit prób logowania. Spróbuj ponownie za minutę.");
                }

                throw new InvalidOperationException("Niepoprawny email lub hasło.");
            }

            if (!user.IsVerified)
            {
                await ResendVerificationEmailAsync(email);
                throw new InvalidOperationException("Konto nie zostało zweryfikowane. Sprawdź swoją skrzynkę e-mail i kliknij link aktywacyjny.");
            }

            await _redisCacheService.RemoveValueAsync(emailAttemptsKey);
            await _redisCacheService.RemoveValueAsync(ipAttemptsKey);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.RoleName)
            };

            return new
            {
                Token = await _tokenService.CreateTokenAsync(claims),
                UserId = user.UserId.ToString()
            };
        }

        /// <summary>
        /// Pobiera użytkownika na podstawie adresu e-mail, wraz z rolą.
        /// </summary>
        /// <param name="email">Adres e-mail użytkownika</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono</returns>
        /// <exception cref="ArgumentException">Jeśli e-mail jest pusty</exception>
        public async Task<User?> GetUserFromEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Podany adres e-mail jest niepoprawny.");
            }

            return await _userRepository.GetUserByEmailWithRoleAsync(email);
        }

        /// <summary>
        /// Wyszukuje użytkownika po jego id
        /// </summary>
        /// <param name="userId">id użytkownika</param>
        /// <returns>Użytkownik</returns>
        /// <exception cref="Exception"></exception>
        public async Task<User> GetUserFromIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new Exception("Niepoprawne id");
            }

            //User? user = await _userRepository.GetUserByIdWithUserLikedMapsAsync(userId);
            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("Bład ze znalezieniem usera o podanym id");

            return user;
        }

        /// <summary>
        /// Pobiera MapsDTO użytkownika
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <returns>Lista obiektów MapsDTO</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<List<MapDto>> GetUserMapsDto(int userId)
        {
            var user = await _userRepository.GetUserByIdWithUserMapsAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("Użytkownik nie znaleziony");
            }

            List<MapDto> maps = new List<MapDto>();

            foreach (var map in user.UserMaps)
            {
                var mapDto = new MapDto()
                {
                    MapId = map.MapId,
                    MapName = map.MapName,
                    MapDescription = map.MapDescription,
                    MapUploaded = map.MapUploaded,
                };

                maps.Add(mapDto);
            }

            return maps;
        }

        /// <summary>
        /// Aktualizuje użytkownika w bazie przez administratora
        /// </summary>
        /// <param name="updateUserDto">nowe dane użytkownika</param>
        public async Task UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
                throw new InvalidDataException("Nieprawidłowe dane zapytania.");

            User? user = await _userRepository.GetUserByIdAsync(updateUserDto.UserId);
            if (user == null)
                throw new InvalidDataException("Nie znaleziono użytkownika o podanym ID.");

            user.UserName = updateUserDto.UserName;
            user.UserEmail = updateUserDto.UserEmail;
            user.UserRoleId = updateUserDto.UserRoleId;

            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Wysyła linka do zresetowania hasła na maila
        /// </summary>
        /// <param name="email">email użytkownika</param>
        public async Task SendResetPasswordLinkAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new InvalidDataException("Błędny email");

            User? user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                throw new InvalidOperationException("Nie znaleziono konta z podanym adresem email.");

            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
            var tokenExpiration = DateTime.UtcNow.AddDays(1);

            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiration = tokenExpiration;

            await _userRepository.UpdateAsync(user);

            var resetLink = $"{_configuration["Siteurl"]}/resetPassword?token={token}";

            var emailSubject = "Resetowanie hasła";
            var emailBody = $"Aby zresetować hasło, kliknij w link: {resetLink}";

            using (var smtpClient = new SmtpClient(_smtpSettings.Host))
            {
                smtpClient.Port = _smtpSettings.Port;
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Email),
                    Subject = emailSubject,
                    Body = emailBody,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(user.UserEmail);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        /// <summary>
        /// Pobiera wszystkich użytkowników
        /// </summary>
        /// <returns>Liste obiektów UserDto</returns>
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users.Select(u => new UserDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                UserEmail = u.UserEmail,
                UserRoleId = (int)u.UserRoleId
            }).ToList();
        }

        /// <summary>
        /// Usuwa użytkownika przez administratora z bazy z jego połączeniami
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        public async Task DeleteUserAsync(int userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidDataException("Nie znaleziono użytkownika o podanym ID.");

            if (user.UserLikedMaps != null && user.UserLikedMaps.Any())
            {
                await _userMapLikeRepository.RemoveRangeAsync(user.UserLikedMaps);
            }

            if (user.UserMaps != null && user.UserMaps.Any())
            {
                foreach (var map in user.UserMaps.ToList())
                {
                    await _mapService.DeleteMapByIdAsync(map.MapId);
                }
            }

            await _userRepository.RemoveAsync(user);
        }

        /// <summary>
        /// Zapisuje zdjęcie profilowe użytkownika
        /// </summary>
        /// <param name="userId">id użytkownika</param>
        /// <param name="imageName">Nazwa zdjęcia z chmury</param>
        public async Task SaveProfileImage(int userId, IFormFile? image)
        {
            string imageName = null;
            if (image != null)
            {
                await _helpers.CheckImageAsync(image);

                imageName = await _firebaseStorageService.UploadFileAsync(image);
            }

            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Nie znaleziono użytkownika o podanym id");

            if (!user.UserProfilePicture.IsNullOrEmpty() && imageName != user.UserProfilePicture)
            {
                await _firebaseStorageService.DeleteFileAsync(user.UserProfilePicture);
            }

            user.UserProfilePicture = imageName;

            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Pobiera zdjęcie profilowe użytkownika
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek z tokenem JWT użytkownika</param>
        /// <returns>Link do zdjęcia profilowego użytkownika</returns>
        public async Task<string> GetUserProfilePicture(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                throw new InvalidDataException("Błąd z tokenem");

            List<Claim> tokenClaims = await _tokenService.GetClaimsFromTokenAsync(authorizationHeader);
            if (tokenClaims == null)
                throw new InvalidDataException("Błędny token");

            int userId = int.Parse(tokenClaims[0].Value);

            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Nie znaleziono użytkownika o podanym id");

            string profilePicture = null;

            if (user.UserProfilePicture != null)
                profilePicture = await _firebaseStorageService.GetSignedUrlAsync(user.UserProfilePicture);

            return profilePicture;
        }

        /// <summary>
        /// Wysyła link na maila z potwierdzeniem konta
        /// </summary>
        /// <param name="user">Obiekt użytkownika</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<User> SendEmailVerificationAsync(User user)
        {
            if (user == null)
                throw new InvalidOperationException("Błędny użytkownik");

            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
            var tokenExpiration = DateTime.UtcNow.AddDays(1);

            user.VerificationToken = token;
            user.VerificationTokenExpiration = tokenExpiration;

            var verifyLink = $"{_configuration["Siteurl"]}/verifyAccount?token={token}";

            var emailSubject = "Potwierdzenie konta";
            var emailBody = $"Aby potwierdzić konto, kliknij w link: {verifyLink}";

            using (var smtpClient = new SmtpClient(_smtpSettings.Host))
            {
                smtpClient.Port = _smtpSettings.Port;
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Email),
                    Subject = emailSubject,
                    Body = emailBody,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(user.UserEmail);
                await smtpClient.SendMailAsync(mailMessage);
                return user;
            }
        }


        /// <summary>
        /// Resetuje hasło użytkownika
        /// </summary>
        /// <param name="request">Zawiera token użytkownika i nowe hasło</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
                throw new InvalidDataException("Niepoprawne dane żądania");

            User? user = await _userRepository.GetUserByTokenAsync(request.Token);

            if (user == null)
                throw new ArgumentNullException("Nie znaleziono użytkownika");

            if (user.ResetPasswordTokenExpiration < DateTime.UtcNow)
                throw new ArgumentNullException("Token jest nieprawidłowy lub wygasł.");

            user.UserPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(request.NewPassword, 13);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiration = null;

            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Potwierdza konto użytkownika po naciściśnięciu w link na mailu
        /// </summary>
        /// <param name="token">Token użytkownika.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Błąd, gdy nie żaden użytkownik nie posiada takiego tokenu</exception>
        public async Task ConfirmVerifyAccountAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Błędny token.");

            User? user = await _userRepository.GetUserByVerificationTokenAsync(token);
            if (user == null)
                throw new Exception("Błędny token.");

            if (user.VerificationTokenExpiration < DateTime.UtcNow)
                throw new Exception("Token wygasł. Proszę ponownie poprosić o link aktywacyjny.");

            user.VerificationToken = null;
            user.VerificationTokenExpiration = null;
            user.IsVerified = true;

            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Wysyła ponownie link do aktywacji konta
        /// </summary>
        /// <param name="email">Email użytkownika</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task ResendVerificationEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Niepoprawny adres e-mail.");

            User? user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                throw new ArgumentException("Nie znaleziono użytkownika.");

            if (user.IsVerified)
                throw new InvalidOperationException("Konto jest już zweryfikowane.");

            string cacheKey = $"ResendVerification:{email}";
            bool canResend = true;

            try
            {
                var lastRequest = await _redisCacheService.GetValueAsync<string>(cacheKey);
                if (lastRequest != null)
                {
                    canResend = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd Redis: {ex.Message}. Ograniczenie e-maili zostanie obsłużone alternatywnie.");
            }

            // Alternatywny mechanizm – sprawdzamy w bazie czas ostatniej prośby
            if (canResend && user.LastVerificationEmailSent != null)
            {
                var timeSinceLastEmail = DateTime.UtcNow - user.LastVerificationEmailSent.Value;
                if (timeSinceLastEmail.TotalMinutes < 5)
                {
                    throw new InvalidOperationException("Prośba o ponowną weryfikację już została wysłana. Spróbuj ponownie za kilka minut.");
                }
            }

            if (!canResend)
                throw new InvalidOperationException("Prośba o ponowną weryfikację już została wysłana. Spróbuj ponownie za kilka minut.");

            // Generujemy nowy token i wysyłamy e-mail
            user = await SendEmailVerificationAsync(user);
            user.LastVerificationEmailSent = DateTime.UtcNow; // Zapisujemy czas wysyłki jako zapas
            await _userRepository.UpdateAsync(user);

            try
            {
                // Ustawiamy limit w Redis na 5 minut (jeśli działa)
                await _redisCacheService.SetValueAsync(cacheKey, "sent", TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu do Redis: {ex.Message}.");
            }
        }



        /// <summary>
        /// Zmienia nazwę użytkownika na nową
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <param name="newUserName">Nowa nazwa użytkownika</param>
        /// <returns></returns>
        public async Task ChangeUserNameAsync(int userId, string newUserName)
        {
            if (userId <= 0)
                throw new ArgumentNullException("Nie udało się zidentyfikować użytkownika.");

            if (string.IsNullOrEmpty(newUserName))
                throw new ArgumentNullException("Nieprawidłowe dane wejściowe.");

            float toxicityScore = await _helpers.AnalyzeText(newUserName);

            if (toxicityScore >= 0.10)
            {
                throw new InvalidOperationException("Nazwa użytkownika nie może być toksyczna.");
            }

            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentNullException("Nie odnaleziono użytkownika.");

            user.UserName = newUserName;
            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Zmienia hasło użytkownika na nowe
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <param name="newUserPassword">nowe hasło</param>
        /// <param name="newUserConfirmPassword">potwierdzenie nowego hasła</param>
        /// <returns></returns>
        public async Task ChangePasswordAsync(int userId, string newUserPassword, string newUserConfirmPassword)
        {
            if (userId <= 0)
                throw new ArgumentNullException("Nie udało się zidentyfikować użytkownika.");

            if (string.IsNullOrEmpty(newUserPassword) || newUserPassword != newUserConfirmPassword)
                throw new ArgumentNullException("Błędne hasło lub potwierdzenie się różni.");

            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentNullException("Nie odnaleziono użytkownika.");

            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(newUserPassword, 13);
            user.UserPassword = hashedPassword;
            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Pobiera ID użytkownika z tokena
        /// </summary>
        /// <param name="token">token użytkownika</param>
        /// <returns>ID użytkownika</returns>
        public async Task<int> GetUserIdFromToken(string token)
        {
            List<Claim> tokenClaims = await _tokenService.GetClaimsFromTokenAsync(token);
            if (tokenClaims == null)
                throw new InvalidDataException("Błędny token");

            return int.Parse(tokenClaims[0].Value);
        }

    }
}
