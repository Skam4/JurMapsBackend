using JurMaps.Model.DTO;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace JurMaps.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/place")]
    public class PlaceController : BaseController<PlaceController>
    {
        private readonly IPlaceService _placeService;
        public PlaceController(IPlaceService placeService, ILogger<PlaceController> logger) : base (null, logger)
        {
            _placeService = placeService;
        }

        /// <summary>
        /// Zapisuje informacje o miejscu.
        /// </summary>
        /// <param name="placeDto">DTO zawierający informacje o miejscu</param>
        /// <returns>Komunikat o statusie operacji</returns>
        [HttpPost("saveplaceinfo")]
        public async Task<IActionResult> SavePlaceInfo([FromForm] PlaceDto placeDto/*[FromHeader(Name = "X-Firebase-AppCheck")] string appCheckToken*/)
        {
            //var verificationResult = await VerifyAppCheckTokenAsync(appCheckToken);

            //if (verificationResult is BadRequestObjectResult)
            //    return BadRequest("Nieudana weryfikacja App Check.");

            try
            {
                await _placeService.SavePlaceInfoAsync(
                    placeDto.PlaceId,
                    placeDto.PlaceName,
                    placeDto.PlaceDescription,
                    placeDto.PlacePhoto1,
                    placeDto.PlacePhoto2,
                    placeDto.PlacePhoto3,
                    placeDto.PlaceColor,
                    placeDto.ChangePlacePhoto1,
                    placeDto.ChangePlacePhoto2,
                    placeDto.ChangePlacePhoto3);

                return Ok(new { success = true, message = "Miejsce zapisane pomyślnie." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w PlaceController.SavePlaceInfo");
                return StatusCode(500, $"Błąd podczas zapisywania miejsca: {ex.Message}");
            }
        }


        /// <summary>
        /// Usuwa miejsce
        /// </summary>
        /// <param name="placeId">ID miejsca</param>
        /// <returns>Komunikat o statusie operacji</returns>
        [HttpDelete("delete/{placeId}")]
        public async Task<ControllerResult<bool>> Delete(int placeId)
        {
            try
            {
                await _placeService.DeleteAsync(placeId);
                return ControllerResult<bool>.Ok(true, "Miejsce usunięte pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w PlaceController.Delete");
                return ControllerResult<bool>.ServerError($"Wystąpił błąd podczas usuwania miejsca: {ex.Message}");
            }
        }
    }
}
