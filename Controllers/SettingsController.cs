using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Services;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace JurMaps.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/settings")]
    public class SettingsController : BaseController<SettingsController>
    {
        IFirebaseStorageService _firebaseStorageService;
        private readonly ITokenService _tokenService;
        public SettingsController(
            IUserService userService,
            ILogger<SettingsController> logger,
            IFirebaseStorageService firebaseStorageService,
            ITokenService tokenService)
            : base(userService, logger)
        {
            _firebaseStorageService = firebaseStorageService; _tokenService = tokenService;
        }

        /// <summary>
        /// Pobiera informacje ustawień użytkownika
        /// </summary>
        /// <param name="authorizationHeader">token użytkownika</param>
        /// <returns>Informacje o ustawieniach użytkownika</returns>
        [HttpGet("getsettingsdata")]
        public async Task<ControllerResult<SettingsDto>> GetSettingsDataAsync([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                var tokenClaims = await _tokenService.GetClaimsFromTokenAsync(authorizationHeader);

                int userId = int.Parse(tokenClaims[0].Value);

                User user = await _userService.GetUserFromIdAsync(userId);

                SettingsDto settingsDto = new SettingsDto
                {
                    UserId = userId,
                    UserName = user.UserName,
                    UserEmail = user.UserEmail
                };

                if(user.UserProfilePicture != null) 
                {
                    settingsDto.UserProfilePicture = await _firebaseStorageService.GetSignedUrlAsync(user.UserProfilePicture);
                }

                return ControllerResult<SettingsDto>.Ok(settingsDto, "Dane użytkownika pobrane pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w SettingsController.GetSettingsDataAsync");
                return ControllerResult<SettingsDto>.BadRequest("Wystąpił błąd podczas pobierania informacji o użytkowniku.");
            }
        }

        /// <summary>
        /// Aktualizuje nazwę użytkownika
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika</param>
        /// <param name="changeUserNameDto">DTO zawierający nową nazwę użytkownika</param>
        /// <returns>Status aktualizacji</returns>
        [HttpPut("changeusername/{userId}")]
        public async Task<ControllerResult<string>> ChangeUserNameAsync(int userId, [FromBody] ChangeUserNameDto changeUserNameDto)
        {
            try
            {
                await _userService.ChangeUserNameAsync(userId, changeUserNameDto.UserName);
                return ControllerResult<string>.Ok("Nazwa użytkownika została zaktualizowana.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w SettingsController.ChangeUserNameAsync");
                return ControllerResult<string>.BadRequest("Wystąpił błąd podczas aktualizowania nazwy użytkownika.");
            }
        }

        /// <summary>
        /// Aktualizuje hasło użytkownika
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika</param>
        /// <param name="changeUserPasswordDto">DTO zawierający nowe hasło i jego potwierdzenie</param>
        /// <returns>Status aktualizacji</returns>
        [HttpPut("changepassword/{userId}")]
        public async Task<ControllerResult<string>> ChangePasswordAsync(int userId, [FromBody] ChangeUserPasswordDto changeUserPasswordDto)
        {
            try
            {
                await _userService.ChangePasswordAsync(userId, changeUserPasswordDto.Password, changeUserPasswordDto.ConfirmPassword);
                return ControllerResult<string>.Ok("Hasło zostało zmienione.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w SettingsController.ChangePasswordAsync");
                return ControllerResult<string>.ServerError("Wystąpił błąd podczas zmiany hasła.");
            }
        }

        /// <summary>
        /// Zapisz obrazek
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <param name="image">adres obrazka</param>
        /// <returns></returns>
        [HttpPut("saveimage/{userId}")]
        public async Task<ControllerResult<string>> SaveImage(int userId, [FromForm] IFormFile? image = null)
        {
            try
            {
                await _userService.SaveProfileImage(userId, image);

                return ControllerResult<string>.Ok("Pomyślnie zapisano zdjęcie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w SettingsController.SaveImage");
                return ControllerResult<string>.ServerError("Wystąpił błąd podczas zapisywania obrazu.");
            }
        }

    }
}
