using JurMaps.Controllers;
using JurMaps.Model;
using JurMaps.Model.DTO;
using System.Threading.Tasks;

namespace JurMaps.Services.Interfaces
{
    public interface IUserService
    {

        /// <summary>
        /// Rejestruje nowego użytkownika w systemie.
        /// </summary>
        /// <param name="model">Obiekt RegisterDto zawierający dane rejestracyjne</param>
        /// <exception cref="ArgumentException">Jeśli dane wejściowe są niepoprawne</exception>
        /// <exception cref="InvalidOperationException">Jeśli konto z podanym e-mailem lub nazwą użytkownika już istnieje</exception>
        Task RegisterUserAsync(RegisterDto model);

        /// <summary>
        /// Loguje użytkownika na podstawie adresu e-mail i hasła.
        /// </summary>
        /// <param name="model">Obiekt LoginDto zawierający dane logowania</param>
        /// <exception cref="ArgumentException">Jeśli dane wejściowe są niepoprawne</exception>
        /// <exception cref="InvalidOperationException">Jeśli e-mail lub hasło są niepoprawne</exception>
        Task<object> LoginUserAsync(LoginDto model, string? ipAddress);

        /// <summary>
        /// Pobiera użytkownika na podstawie adresu e-mail, wraz z rolą.
        /// </summary>
        /// <param name="email">Adres e-mail użytkownika</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono</returns>
        /// <exception cref="ArgumentException">Jeśli e-mail jest pusty</exception>
        Task<User?> GetUserFromEmailAsync(string email);

        /// <summary>
        /// Pobiera MapsDTO użytkownika
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <returns>Lista obiektów MapsDTO</returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task<List<MapDto>> GetUserMapsDto(int userId);
        /// <summary>
        /// Wyszukuje użytkownika po jego ID
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <returns>Użytkownik</returns>
        /// <exception cref="Exception"></exception>
        Task<User> GetUserFromIdAsync(int userId);

        /// <summary>
        /// Aktualizuje użytkownika w bazie przez administratora
        /// </summary>
        /// <param name="updateUserDto">nowe dane użytkownika</param>
        Task UpdateUserAsync(UpdateUserDto updateUserDto);

        /// <summary>
        /// Wysyła linka do zresetowania hasła na maila
        /// </summary>
        /// <param name="email">email użytkownika</param>
        Task SendResetPasswordLinkAsync(string email);

        /// <summary>
        /// Pobiera wszystkich użytkowników
        /// </summary>
        /// <returns>Liste obiektów UserDto</returns>
        Task<List<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// Usuwa użytkownika przez administratora z bazy z jego połączeniami
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        Task DeleteUserAsync(int userId);

        /// <summary>
        /// Zapisuje zdjęcie profilowe użytkownika
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <param name="image">Plik ze zdjęciem</param>
        Task SaveProfileImage(int userId, IFormFile image);

        /// <summary>
        /// Pobiera zdjęcie profilowe użytkownika
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek z tokenem JWT użytkownika</param>
        /// <returns>Link do zdjęcia profilowego użytkownika</returns>
        Task<string> GetUserProfilePicture(string authorizationHeader);

        /// <summary>
        /// Resetuje hasło użytkownika
        /// </summary>
        /// <param name="request">Zawiera token użytkownika i nowe hasło</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request);

        /// <summary>
        /// Potwierdza konto użytkownika po naciściśnięciu w link na mailu
        /// </summary>
        /// <param name="token">Token użytkownika.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Błąd, gdy nie żaden użytkownik nie posiada takiego tokenu</exception>
        Task ConfirmVerifyAccountAsync(string token);

        /// <summary>
        /// Zmienia nazwę użytkownika na nową
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <param name="newUserName">Nowa nazwa użytkownika</param>
        /// <returns></returns>
        Task ChangeUserNameAsync(int userId, string newUserName);

        /// <summary>
        /// Zmienia hasło użytkownika na nowe
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <param name="newUserPassword">nowe hasło</param>
        /// <param name="newUserConfirmPassword">potwierdzenie nowego hasła</param>
        /// <returns></returns>
        Task ChangePasswordAsync(int userId, string newUserPassword, string newUserConfirmPassword);

        /// <summary>
        /// Wysyła ponownie link do aktywacji konta
        /// </summary>
        /// <param name="email">Email użytkownika</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        Task ResendVerificationEmailAsync(string email);

        /// <summary>
        /// Pobiera ID użytkownika z tokena
        /// </summary>
        /// <param name="token">token użytkownika</param>
        /// <returns>ID użytkownika</returns>
        Task<int> GetUserIdFromToken(string token);
    }
}
