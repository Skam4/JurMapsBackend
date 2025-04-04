using Google.Apis.Logging;
using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Repository.Interfaces;
using JurMaps.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JurMaps.Services
{
    public class TokenService : ITokenService
    {
        private IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public TokenService(IConfiguration configuration, IUserRepository userRepository)   
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Odświeżanie tokenu
        /// </summary>
        /// <param name="authorizationHeader">Header z tokenem użytkownika</param>
        /// <returns>Token i identyfikator użytkownika</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<TokenRefreshResponse> RefreshTokenAsync(string authorizationHeader)
        {
            try
            {
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                    throw new InvalidDataException("Token nieprawidłowy");

                var tokenClaims = await GetClaimsFromTokenAsync(authorizationHeader);
                if (tokenClaims.Count == 0)
                    throw new InvalidDataException("Brak poprawnych claimsów w tokenie");

                var response = new TokenRefreshResponse
                {
                    Token = await CreateTokenAsync(tokenClaims),
                    UserId = tokenClaims[0].Value
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas odświeżania tokenu: {ex.Message}");
            }
        }

        /// <summary>
        /// Wyciąga z tokena dane i sprawdza czy jego rola to admin
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek z tokenem JWT użytkownika</param>
        /// <returns>True, jeżeli rola w podanym tokenie to admin</returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<bool> CheckIfUserIsAdminAsync(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                throw new InvalidDataException("Błąd z tokenem.");

            List<Claim> tokenClaims = await GetClaimsFromTokenAsync(authorizationHeader);

            if (tokenClaims == null)
                throw new Exception("Błędny token.");

            bool isAdmin = tokenClaims[1].Value.Equals("admin", StringComparison.OrdinalIgnoreCase);
            return isAdmin;
        }

        /// <summary>
        /// Pobiera listę roszczeń (claims) z tokena JWT zawartego w nagłówku autoryzacyjnym.
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek autoryzacyjny w formacie "Bearer {token}"</param>
        /// <returns>Lista roszczeń (claims) lub pusta lista, jeśli token jest niepoprawny</returns>
        public Task<List<Claim>> GetClaimsFromTokenAsync(string authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return Task.FromResult(new List<Claim>());
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(new List<Claim>());
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return Task.FromResult(jwtToken?.Claims?.ToList() ?? new List<Claim>());
            }
            catch (Exception)
            {
                return Task.FromResult(new List<Claim>());
            }
        }

        /// <summary>
        /// Tworzy nowy token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns>Nowy token</returns>
        /// <exception cref="InvalidOperationException">Brak konfiguracji tokenu</exception>
        public Task<string> CreateTokenAsync(List<Claim> claims)
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(tokenString);
        }

        /// <summary>
        /// Sprawdza, czy token jest aktualny i czy użytkownik istnieje w systemie.
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek autoryzacyjny zawierający token</param>
        /// <returns>True, jeśli token jest ważny i użytkownik istnieje</returns>
        /// <exception cref="InvalidDataException">Nieprawidłowy lub przeterminowany token</exception>
        public async Task<bool> IsTokenValidAndUserExistsAsync(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new InvalidDataException("Nieprawidłowy token.");
            }

            var tokenClaims = await GetClaimsFromTokenAsync(authorizationHeader);
            if (tokenClaims == null || tokenClaims.Count == 0)
            {
                throw new InvalidDataException("Token nie zawiera poprawnych danych.");
            }

            var userIdClaim = tokenClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new InvalidDataException("Token nie zawiera identyfikatora użytkownika.");
            }

            var user = await _userRepository.GetUserByIdAsync(int.Parse(userIdClaim.Value));
            if (user == null)
            {
                throw new InvalidDataException("Użytkownik nie istnieje.");
            }

            return true;
        }

    }

}
