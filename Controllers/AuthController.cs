using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Services;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RTools_NTS.Util;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace JurMaps.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    public class AuthController : BaseController<AuthController>
    {
        private readonly ITokenService _tokenService;

        public AuthController(
            ILogger<AuthController> logger,
            IUserService userService,
            ITokenService tokenService)
            : base(userService, logger)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Rejestracja nowego użytkownika.
        /// </summary>
        /// <param name="model">DTO zawierający dane rejestracyjne</param>
        /// <returns>Komunikat o statusie operacji</returns>
        [HttpPost("registeruser")]
        public async Task<ControllerResult<string>> RegisterUser([FromBody] RegisterDto model)
        {
            try
            {
                await _userService.RegisterUserAsync(model);
                return ControllerResult<string>.Ok("Użytkownik został pomyślnie utworzony");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.RegisterUser");
                return ControllerResult<string>.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.RegisterUser");
                return ControllerResult<string>.BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Logowanie użytkownika.
        /// </summary>
        /// <param name="model">DTO zawierający dane logowania</param>
        /// <returns>Token i identyfikator użytkownika</returns>
        [HttpPost("loginuser")]
        public async Task<ControllerResult<object>> LoginUser([FromBody] LoginDto model)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            try
            {
                var loginResult = await _userService.LoginUserAsync(model, ipAddress);
                return ControllerResult<object>.Ok(loginResult);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Błąd w AuthController.LoginUser");
                return ControllerResult<object>.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Błąd w AuthController.LoginUser");
                return ControllerResult<object>.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Odświeżenie tokenu
        /// </summary>
        /// <param name="authorizationHeader">token</param>
        /// <returns>Token i identyfikator użytkownika</returns>
        [HttpPost("refresh-token")]
        public async Task<ControllerResult<TokenRefreshResponse>> RefreshToken([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                return ControllerResult<TokenRefreshResponse>.Ok(await _tokenService.RefreshTokenAsync(authorizationHeader));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.RefreshToken");
                return ControllerResult<TokenRefreshResponse>.BadRequest($"Błąd podczas odświeżania tokenu: {ex.Message}");
            }
        }

        /// <summary>
        /// Wysyłanie linka do resetowania hasła użytkownika
        /// </summary>
        /// <param name="email">email</param>
        [HttpPost("resetpassword")] //Można zmienić nazwe routa, żeby była bardziej precyzyjna - sendlinktoresetpassword
        public async Task<ControllerResult<string>> ResetPassword([FromBody] string email)
        {
            try
            {
                await _userService.SendResetPasswordLinkAsync(email);
                return ControllerResult<string>.Ok("Link do resetowania hasła został wysłany.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.ResetPassword");
                return ControllerResult<string>.BadRequest($"Błąd podczas wysyłania linka do resetowania hasła: {ex.Message}");
            }
        }

        /// <summary>
        /// Potwierdzenie resetowania hasła użytkownika
        /// </summary>
        /// <param name="request">Zawiera token użytkownika i nowe hasło</param>
        /// <returns></returns>
        [HttpPost("confirmresetpassword")]
        public async Task<ControllerResult<string>> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequest request)
        {
            try
            {
                await _userService.ConfirmResetPasswordAsync(request);
                return ControllerResult<string>.Ok("Hasło zostało pomyślnie zresetowane.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.ConfirmResetPassword");
                return ControllerResult<string>.BadRequest($"Błąd podczas potwierdzenia resetowania hasła: {ex.Message}");
            }
        }

        /// <summary>
        /// Sprawdza czy użytkownik jest administratorem
        /// </summary>
        /// <param name="authorizationHeader">Token użytkownika</param>
        /// <returns></returns>
        [HttpGet("checkifuserisadmin")]
        public async Task<ControllerResult<bool>> CheckIfUserIsAdmin([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                return ControllerResult<bool>.Ok(await _tokenService.CheckIfUserIsAdminAsync(authorizationHeader));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.CheckIfUserIsAdmin");
                return ControllerResult<bool>.BadRequest($"Błąd podczas sprawdzania czy użytkownik jest adminem: {ex.Message}");
            }
        }

        /// <summary>
        /// Pobiera zdjęcie profilowe użytkownika
        /// </summary>
        /// <param name="authorizationHeader">Token użytkownika</param>
        /// <returns>Link ze zdjęciem</returns>
        [HttpGet("getuserprofilepicture")]
        public async Task<ControllerResult<string>> GetUserProfilePicture([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                string profilePicture = await _userService.GetUserProfilePicture(authorizationHeader);
                return ControllerResult<string>.Ok(profilePicture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.GetUserProfilePicture");
                return ControllerResult<string>.BadRequest($"Błąd podczas pobierania zdjęcia profilowego użytkownika: {ex.Message}");
            }
        }

        /// <summary>
        /// Potwierdza konto użytkownika
        /// </summary>
        /// <param name="token">Token użytkownika</param>
        /// <returns></returns>
        [HttpPost("confirmverifyaccount")]
        public async Task<ControllerResult<bool>> ConfirmVerifyAccount([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                await _userService.ConfirmVerifyAccountAsync(authorizationHeader);
                return ControllerResult<bool>.Ok(true, "Pomyślnie zatwierdzono konto użytkownika");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.ConfirmVerifyAccount");
                return ControllerResult<bool>.BadRequest($"Błąd podczas potwierdzania konta użytkownika: {ex.Message}");
            }
        }

        /// <summary>
        /// Wysyła ponownie link do weryfikacji emaila
        /// </summary>
        /// <param name="model">Przechowuje email użytkownika</param>
        /// <returns>True, jeżeli operacja przeszła pomyślnie</returns>
        [HttpPost("resend-verification")]
        public async Task<ControllerResult<bool>> ResendVerificationEmail([FromBody] ResendVerificationDto model)
        {
            try
            {
                await _userService.ResendVerificationEmailAsync(model.Email);
                return ControllerResult<bool>.Ok(true, "Nowy link weryfikacyjny został wysłany.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.ResendVerificationEmail");
                return ControllerResult<bool>.BadRequest($"Błąd podczas wysyłania ponownie linka weryfikującego konto: {ex.Message}");
            }
        }

        [HttpPost("getusercontextdata")]
        public async Task<ControllerResult<UserContextDto>> GetUserContextData([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                UserContextDto userContextDto = new UserContextDto
                {
                    profilePicture = await _userService.GetUserProfilePicture(authorizationHeader),
                    isAdmin = await _tokenService.CheckIfUserIsAdminAsync(authorizationHeader),
                    isLogged = await _tokenService.IsTokenValidAndUserExistsAsync(authorizationHeader),
                    userId = await _userService.GetUserIdFromToken(authorizationHeader)
                };
                return ControllerResult<UserContextDto>.Ok(userContextDto, "Dane użytkownika pobrane pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AuthController.GetUserContextData");
                return ControllerResult<UserContextDto>.BadRequest($"Błąd podczas pobierania danych użytkownika: {ex.Message}");
            }


        }
    }

}
