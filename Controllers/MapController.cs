using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Services;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JurMaps.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    public class MapController : BaseController<MapController>
    {
        private readonly IUserMapLikesService _userMapLikesService;
        public MapController(
            IMapService mapService, 
            IUserService userService, 
            IUserMapLikesService userMapLikesService, 
            ILogger<MapController> logger)
            : base(userService, logger, mapService)
        {
            _userMapLikesService = userMapLikesService;
        }

        /// <summary>
        /// Pobiera informacje o mapie po id mapy
        /// </summary>
        /// <param name="mapId">Identifikator mapy</param>
        /// <returns>Obiekt z informacjami mapy</returns>
        [HttpGet("getmapinfo/{mapId}")]
        public async Task<ControllerResult<MapDto>> GetMapInfo(int mapId)
        {
            try
            {
                MapDto mapDto = await _mapService.GetMapDtoByIdAsync(mapId);
                return ControllerResult<MapDto>.Ok(mapDto, "Pomyślnie pobrano dane o mapie");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapController.GetMapInfo");
                return ControllerResult<MapDto>.ServerError("Wystąpił błąd podczas pobierania informacji o mapie.");
            }
        }

        /// <summary>
        /// Dodaje polubienie do mapy przez danego użytkownika.
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="likeDto">DTO zawierający ID użytkownika</param>
        /// <returns>Komunikat o statusie</returns>
        [HttpPut("addlike/{mapId}")]
        public async Task<ControllerResult<string>> AddLike(int mapId, [FromBody] LikeDto likeDto)
        {
            try
            {
                await _userMapLikesService.AddLikeAsync(mapId, likeDto.UserId);
                return ControllerResult<string>.Ok("Mapa została polubiona.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapController.AddLike");
                return ControllerResult<string>.ServerError("Wystąpił błąd podczas dodawania polubienia.");
            }
        }

        /// <summary>
        /// Sprawdza, czy użytkownik polubił mapę.
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="userId">ID użytkownika</param>
        /// <returns>True, jeśli użytkownik polubił mapę, w przeciwnym razie false</returns>
        [HttpGet("hasliked/{mapId}/{userId}")]
        public async Task<ControllerResult<bool>> HasLiked(int mapId, int userId)
        {
            try
            {
                UserMapLike? hasLiked = await _userMapLikesService.WasMapLikedByUserAsync(mapId, userId);
                if (hasLiked == null)
                    return ControllerResult<bool>.Ok(false, "Użytkownik nie polubił mapy.");

                return ControllerResult<bool>.Ok(true, "Użytkownik polubił mapę.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapController.HasLiked");
                return ControllerResult<bool>.ServerError("Wystąpił błąd podczas sprawdzania polubienia.");
            }
        }

        /// <summary>
        /// Usuwa polubienie z mapy przez danego użytkownika.
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="likeDto">DTO zawierający ID użytkownika</param>
        /// <returns>Komunikat o statusie</returns>
        [HttpPut("removelike/{mapId}")]
        public async Task<ControllerResult<string>> RemoveLike(int mapId, [FromBody] LikeDto likeDto)
        {
            try
            {
                await _userMapLikesService.RemoveLikeAsync(mapId, likeDto.UserId);
                return ControllerResult<string>.Ok("Polubienie zostało usunięte.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapController.RemoveLike");
                return ControllerResult<string>.ServerError("Wystąpił błąd podczas usuwania polubienia.");
            }
        }

        /// <summary>
        /// Usuwa mapę z bazy
        /// </summary>
        /// <param name="mapId">id mapy</param>
        [HttpDelete("deletemap/{mapId}")]
        public async Task<ControllerResult<string>> DeleteMapByIdAsync(int mapId)
        {
            try
            {
                await _mapService.DeleteMapByIdAsync(mapId);
                return ControllerResult<string>.Ok("Usunięto mapę pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapController.DeleteMapByIdAsync");
                return ControllerResult<string>.ServerError("Wystąpił błąd podczas usuwania mapy.");
            }
        }
    }

}
