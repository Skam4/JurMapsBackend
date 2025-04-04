using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace JurMaps.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/profile")]
    public class ProfileController : BaseController<ProfileController>
    {
        private readonly ITokenService _tokenService;
        private readonly IUserMapLikesService _userMapLikes;
        private readonly IFirebaseStorageService _firebaseStorageService;

        public ProfileController(
            IUserService userService,
            IMapService mapService,
            ITokenService tokenService,
            IUserMapLikesService userMapLikes,
            ILogger<ProfileController> logger,
            IFirebaseStorageService firebaseStorageService)
            : base(userService, logger, mapService)
        {
            _tokenService = tokenService;
            _userMapLikes = userMapLikes;
            _firebaseStorageService = firebaseStorageService;
        }

        /// <summary>
        /// Pobiera dane profilu użytkownika na podstawie jego ID.
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <returns>Dane profilu użytkownika</returns>
        [HttpGet("getuserdata/{userId}")]
        public async Task<ControllerResult<ProfileDto>> GetUserData(int userId)
        {
            if (userId <= 0)
                return ControllerResult<ProfileDto>.BadRequest("Nie podano ID użytkownika.");

            try
            {
                User user = await _userService.GetUserFromIdAsync(userId);

                ProfileDto profileDto = new ProfileDto
                {
                    UserName = user.UserName,
                    UserCreatedDate = user.UserCreatedDate
                };

                if(user.UserProfilePicture != null)
                {
                    profileDto.UserProfilePicture = await _firebaseStorageService.GetSignedUrlAsync(user.UserProfilePicture);
                }

                return ControllerResult<ProfileDto>.Ok(profileDto, "Poprawnie pobrano dane profilu użytkownika");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.GetUserData");
                return ControllerResult<ProfileDto>.BadRequest("Wystąpił błąd podczas pobierania opublikowanych map.");
            }
        }

        /// <summary>
        /// Pobiera opublikowane mapy względem page
        /// </summary>
        /// <param name="profileId">id użytkownika</param>
        /// <param name="page">Określa ile rekordów pominąć przy pobieraniu danych</param>
        /// <returns>Lista map</returns>
        [HttpGet("getuserpublicatedmaps")]
        public async Task<ControllerResult<List<MapDto>>> GetPublicatedMaps([FromQuery] string profileId, [FromQuery] string page)
        {
            try
            {
                List<MapDto> userPublicatedMaps = await _mapService.GetPublicatedMapsByUserIdAsyncAndPageNumber(profileId, page);
                return ControllerResult<List<MapDto>>.Ok(userPublicatedMaps);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.GetPublicatedMaps");
                return ControllerResult<List<MapDto>>.BadRequest("Wystąpił błąd podczas pobierania opublikowanych map.");
            }
        }

        /// <summary>
        /// Pobiera nieopublikowane mapy względem page
        /// </summary>
        /// <param name="profileId">id użytkownika</param>
        /// <param name="page">Określa ile rekordów pominąć przy pobieraniu danych</param>
        /// <returns>Lista map</returns>
        [HttpGet("getuserunpublishedmaps")]
        public async Task<ControllerResult<List<MapDto>>> GetPrivateMaps([FromQuery] string profileId, [FromQuery] string page)
        {
            try
            {
                List<MapDto> userPrivateMaps = await _mapService.GetPrivateMapsByUserIdAsyncAndPageNumber(profileId, page);
                return ControllerResult<List<MapDto>>.Ok(userPrivateMaps);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.GetPrivateMaps");
                return ControllerResult<List<MapDto>>.BadRequest("Wystąpił błąd podczas pobierania nieopublikowanych map.");
            }
        }

        /// <summary>
        /// Pobiera polubione mapy względem page
        /// </summary>
        /// <param name="profileId">id użytkownika</param>
        /// <param name="page">Określa ile rekordów pominąć przy pobieraniu danych</param>
        /// <returns>Lista map</returns>
        [HttpGet("getuserlikedmaps")]
        public async Task<ControllerResult<List<MapDto>>> GetLikedMaps([FromQuery] string profileId, [FromQuery] string page)
        {
            try
            {
                List<MapDto> userLikedMaps = await _userMapLikes.GetLikedMapsByUserIdAndPageNumberAsync(profileId, page);
                return ControllerResult<List<MapDto>>.Ok(userLikedMaps);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.GetLikedMaps");
                return ControllerResult<List<MapDto>>.BadRequest("Wystąpił błąd podczas pobierania polubionych map.");
            }
        }

        /// <summary>
        /// Pobiera ID użytkownika na podstawie tokenu autoryzacyjnego.
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek zawierający token autoryzacyjny</param>
        /// <returns>ID użytkownika</returns>
        [HttpGet("getuseridfromtoken")]
        public async Task<ControllerResult<string>> GetUserIdFromToken([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                var tokenClaims = await _tokenService.GetClaimsFromTokenAsync(authorizationHeader);
                return ControllerResult<string>.Ok(tokenClaims[0].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.GetUserIdFromToken");
                return ControllerResult<string>.BadRequest("Wystąpił błąd podczas pobierania ID użytkownika z tokenu.");
            }
        }

        /// <summary>
        /// Pobiera prywatne mapy użytkownika
        /// </summary>
        /// <param name="userId">ID użytkownika.</param>
        /// <returns>Lista z mapami</returns>
        [HttpGet("getprivatemaps/{userId}")]
        public async Task<ControllerResult<List<MapDto>>> GetPrivateMapsAsync(string userId)
        {
            try
            {
                List<MapDto> privateMaps = await _mapService.GetPrivateMapsByUserIdAsync(userId);
                return ControllerResult<List<MapDto>>.Ok(privateMaps);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.GetPrivateMapsAsync");
                return ControllerResult<List<MapDto>>.BadRequest("Wystąpił błąd podczas pobierania prywatnych map użytkownika.");
            }
        }

        /// <summary>
        /// Usuwa mapę
        /// </summary>
        /// <param name="id">ID mapy.</param>
        /// <returns>Komunikat o zakończeniu operacji</returns>
        [HttpDelete("deletemap/{id}")]
        public async Task<ControllerResult<string>> DeleteMapByIdAsync(int id)
        {
            try
            {
                await _mapService.DeleteMapByIdAsync(id);
                return ControllerResult<string>.Ok("Usunięto mapę pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.DeleteMapByIdAsync");
                return ControllerResult<string>.BadRequest("Wystąpił błąd podczas usuwania mapy.");
            }
        }

        /// <summary>
        /// Publikuje mapę
        /// </summary>
        /// <param name="id">ID mapy.</param>
        /// <returns>Komunikat o zakończeniu operacji</returns>
        [HttpPut("publishmap/{id}")]
        public async Task<ControllerResult<string>> PublishMap(int id)
        {
            try
            {
                await _mapService.PublishMapAsync(id);
                return ControllerResult<string>.Ok("Opublikowano mapę pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.PublishMap");
                return ControllerResult<string>.BadRequest("Wystąpił błąd podczas publikowania mapy.");
            }
        }

        /// <summary>
        /// Przenosi mapę z opublikowanej do wersji roboczej
        /// </summary>
        /// <param name="id">ID mapy</param>
        /// <returns>Komunikat o zakończeniu operacji</returns>
        [HttpPut("movemaptodraft/{id}")]
        public async Task<ControllerResult<string>> MoveMapToDraft(int id)
        {
            try
            {
                await _mapService.MoveMapToDraftAsync(id);
                return ControllerResult<string>.Ok("Przeniesiono mapę pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w ProfileController.MoveMapToDraft");
                return ControllerResult<string>.BadRequest("Wystąpił błąd podczas przenoszenia mapy do wersji roboczej.");
            }
        }
    }
}
