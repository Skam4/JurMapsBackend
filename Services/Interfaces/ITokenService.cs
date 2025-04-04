using JurMaps.Model.DTO;
using System.Security.Claims;

namespace JurMaps.Services.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Odświeżanie tokenu
        /// </summary>
        /// <param name="authorizationHeader">Header z tokenem użytkownika</param>
        /// <returns>Token i identyfikator użytkownika</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        Task<TokenRefreshResponse> RefreshTokenAsync(string authorizationHeader);

        /// <summary>
        /// Wyciąga z tokena dane i sprawdza czy jego rola to admin
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek z tokenem JWT użytkownika</param>
        /// <returns>True, jeżeli rola w podanym tokenie to admin</returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="Exception"></exception>
        Task<bool> CheckIfUserIsAdminAsync(string authorizationHeader);

        /// <summary>
        /// Pobiera listę roszczeń (claims) z tokena JWT zawartego w nagłówku autoryzacyjnym.
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek autoryzacyjny w formacie "Bearer {token}"</param>
        /// <returns>Lista roszczeń (claims) lub pusta lista, jeśli token jest niepoprawny</returns>
        Task<List<Claim>> GetClaimsFromTokenAsync(string authorizationHeader);

        /// <summary>
        /// Tworzy nowy token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns>Nowy token</returns>
        /// <exception cref="InvalidOperationException">Brak konfiguracji tokenu</exception>
        Task<string> CreateTokenAsync(List<Claim> claims);

        /// <summary>
        /// Sprawdza, czy token jest aktualny i czy użytkownik istnieje w systemie.
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek autoryzacyjny zawierający token</param>
        /// <returns>True, jeśli token jest ważny i użytkownik istnieje</returns>
        /// <exception cref="InvalidDataException">Nieprawidłowy lub przeterminowany token</exception>
        Task<bool> IsTokenValidAndUserExistsAsync(string authorizationHeader);
    }
}
