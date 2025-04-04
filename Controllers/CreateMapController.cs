using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;

namespace JurMaps.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    public class CreateMapController : BaseController<CreateMapController>
    {

        private readonly IPlaceService _placeService;
        private readonly ITokenService _tokenService;
        private readonly ITagService _tagService;
        private readonly ICountryService _countryService;

        public CreateMapController(
            IPlaceService placeService,
            ITokenService tokenService,
            IUserService userService,
            IMapService mapService,
            ITagService tagService,
            ICountryService countryService,
            ILogger<CreateMapController> logger)
            : base (userService, logger, mapService)
        {
            _placeService = placeService;
            _tokenService = tokenService;
            _tagService = tagService;
            _countryService = countryService;
        }

        /// <summary>
        /// Dodaje stworzony marker do bazy
        /// </summary>
        /// <param name="marker">Obiekt markera</param>
        /// <param name="mapId">ID mapy</param>
        /// <returns></returns>
        [HttpPost("addmarker/{mapId}")]
        public async Task<ControllerResult<int>> AddMarker(int mapId, [FromBody] MarkerDto marker)
        {
            try
            {
                int placeId = await _placeService.AddMarkerAsync(marker, mapId);
                return ControllerResult<int>.Ok(placeId, "Marker został dodany prawidłowo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.AddMarker");
                return ControllerResult<int>.ServerError($"Wystąpił błąd podczas dodawania markera: {ex.Message}");
            }
        }

        /// <summary>
        /// Tworzy nową mapę na podstawie tokenu autoryzacyjnego.
        /// </summary>
        /// <param name="authorizationHeader">Nagłówek zawierający token autoryzacyjny</param>
        /// <returns>ID nowo utworzonej mapy</returns>
        [HttpPost("createmap")]
        public async Task<ControllerResult<int>> CreateMap([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                int userId = await GetUserIdFromToken(authorizationHeader);
                if (userId <= 0)
                    return ControllerResult<int>.BadRequest("Nieprawidłowe dane tokena.");

                int newMapId = await _mapService.SaveMapAndReturnIdAsync(userId);
                return ControllerResult<int>.Ok(newMapId, "Pomyślnie utworzono mapę.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ControllerResult<int>.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.CreateMap");
                return ControllerResult<int>.ServerError(ex.Message);
            }
        }

        /// <summary>
        /// Pobiera markery mapy na podstawie ID mapy.
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <returns>Lista markerów mapy</returns>
        [HttpGet("getmapmarkers/{mapId}")]
        public async Task<ControllerResult<List<MarkerDto>>> GetMapMarkers(int mapId)
        {
            try
            {
                List<MarkerDto> mapMarkers = await _mapService.GetMapMarkers(mapId);
                return ControllerResult<List<MarkerDto>>.Ok(mapMarkers, "Pobrano listę markerów.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.GetMapMarkers");
                return ControllerResult<List<MarkerDto>>.ServerError($"Błąd podczas pobierania markerów: {ex.Message}");
            }
        }

        /// <summary>
        /// Pobiera okręgi mapy na podstawie ID mapy.
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <returns>Lista okręgów mapy</returns>
        [HttpGet("getmapcircles/{mapId}")]
        public async Task<ControllerResult<List<CircleDto>>> GetMapCircles(int mapId)
        {
            try
            {
                List<CircleDto> circleList = await _mapService.GetMapCircles(mapId);
                return ControllerResult<List<CircleDto>>.Ok(circleList, "Pomyślnie pobrano okręgi mapy.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.GetMapCircles");
                return ControllerResult<List<CircleDto>>.ServerError($"Wystąpił błąd podczas pobierania okręgów: {ex.Message}");
            }
        }

        /// <summary>
        /// Metoda zapisująca nową mapę do bazy.
        /// </summary>
        /// <param name="mapDto">Obiekt zawierający dane nowej mapy</param>
        /// <param name="imageFile">plik zdjęcia miniaturki mapy</param>
        /// <returns>Status operacji</returns>
        [HttpPost("savemap")]
        public async Task<ControllerResult<string>> SaveMap([FromForm] string mapDto, [FromForm] IFormFile? imageFile)
        {
            try
            {
                var mapDtoObj = JsonConvert.DeserializeObject<MapDto>(mapDto);

                await _mapService.SaveMapAsync(mapDtoObj, imageFile);
                return ControllerResult<string>.Ok("Mapa dodana pomyślnie.");
            }
            catch (InvalidDataException ex)
            {
                _logger.LogWarning(ex, "Nie znaleziono mapy");
                return ControllerResult<string>.NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Błędne dane wejściowe");
                return ControllerResult<string>.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapController.SaveMap");
                return ControllerResult<string>.ServerError($"Wystąpił błąd podczas zapisywania mapy: {ex.Message}");
            }
        }

        /// <summary>
        /// Pobiera dane mapy na podstawie jej ID.
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <returns>Dane mapy</returns>
        [HttpGet("getmapdata/{mapId}")]
        public async Task<ControllerResult<MapDto>> GetMapData(int mapId)
        {
            try
            {
                MapDto mapData = await _mapService.GetMapDtoByIdAsync(mapId);
                return ControllerResult<MapDto>.Ok(mapData, "Pomyślnie pobrano mapę.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.GetMapData");
                return ControllerResult<MapDto>.ServerError($"Wystąpił błąd podczas pobierania informacji mapy: {ex.Message}");
            }
        }

        /// <summary>
        /// Aktualizuje pozycję markera.
        /// </summary>
        /// <param name="markerId">ID markera</param>
        /// <param name="position">Nowa pozycja markera</param>
        /// <returns>Status operacji</returns>
        [HttpPut("updatemarkerposition/{markerId}")]
        public async Task<ControllerResult<string>> UpdateMarkerPosition(int markerId, [FromBody] PositionDto position)
        {
            try
            {
                await _placeService.UpdateMarkerPositionAsync(markerId, position);
                return ControllerResult<string>.Ok("Pozycja markera została zaktualizowana.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.UpdateMarkerPosition");
                return ControllerResult<string>.ServerError($"Błąd podczas aktualizacji markera: {ex.Message}");
            }
        }

        /// <summary>
        /// Dodaje kraj do mapy.
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="countryName">Nazwa kraju</param>
        /// <returns>Status operacji</returns>

        [HttpPost("addmapcountry/{mapId}")]
        public async Task<ControllerResult<string>> AddMapCountry(int mapId, [FromBody] string countryName)
        {
            try
            {
                await _countryService.AddMapCountryAsync(mapId, countryName);
                return ControllerResult<string>.Ok("Pomyślnie dodano kraj do mapy.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.AddMapCountry");
                return ControllerResult<string>.ServerError($"Wystąpił błąd podczas dodawania kraju do mapy: {ex.Message}");
            }
        }

        /// <summary>
        /// Sprawdza czy mapa należy do użytkownika
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="authorizationHeader">Token użytkownika</param>
        /// <returns>Informacje czy mapa należy do użytkownika</returns>
        [HttpGet("verifyuser/{mapId}")]
        public async Task<ControllerResult<string>> VerifyUser(int mapId, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                int? userId = await GetUserIdFromToken(authorizationHeader);
                if (userId <= 0)
                    return ControllerResult<string>.BadRequest("Nieprawidłowe dane tokena.");

                Map map = await _mapService.GetMapByIdAsync(mapId);
                if (map == null)
                    return ControllerResult<string>.NotFound("Mapa nie istnieje.");

                return map.MapCreatorId == userId
                    ? ControllerResult<string>.Ok("Mapa należy do użytkownika.")
                    : ControllerResult<string>.NotFound("Mapa nie należy do użytkownika.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.VerifyUser");
                return ControllerResult<string>.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w CreateMapController.VerifyUser");
                return ControllerResult<string>.ServerError($"Błąd podczas weryfikowania użytkownika: {ex.Message}");
            }
        }

        /// <summary>
        /// Sprawdza ID z tokenu użytkownika
        /// </summary>
        /// <param name="authorizationHeader">Token użytkownika</param>
        /// <returns>ID użytkownika</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        private async Task<int> GetUserIdFromToken(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader))
                throw new UnauthorizedAccessException("Brak tokena w nagłówku.");

            var tokenClaims = await _tokenService.GetClaimsFromTokenAsync(authorizationHeader);
            if (tokenClaims == null)
                throw new UnauthorizedAccessException("Nieprawidłowe dane tokena.");

            Claim? idClaim = tokenClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Nie znaleziono identyfikatora w tokenie.");

            return userId;
        }


    }
}
