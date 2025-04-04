using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace JurMaps.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    public class MapsController : BaseController<MapsController>
    {
        private readonly ITagService _tagService;
        private readonly ICountryService _countryService;

        public MapsController(
            IMapService mapService, 
            ITagService tagService, 
            ICountryService countryService, 
            ILogger<MapsController> logger)
            : base(null, logger, mapService)
        {
            _tagService = tagService;
            _countryService = countryService;
        }

        /// <summary>
        /// Pobiera wszystkie mapy.
        /// </summary>
        /// <returns>Lista map</returns>
        [HttpGet("getmaps")]
        public async Task<ControllerResult<List<MapDto>>> GetMaps()
        {
            try
            {
                List<MapDto> mapList = await _mapService.GetMapsAsync();
                return ControllerResult<List<MapDto>>.Ok(mapList, "Pobrano listę map.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapsController.GetMaps");
                return ControllerResult<List<MapDto>>.ServerError("Wystąpił błąd podczas pobierania wszystkich map.");
            }
        }

        /// <summary>
        /// Pobiera wszystkie opublikowane mapy.
        /// </summary>
        /// <returns>Lista opublikowanych map</returns>
        [HttpGet("getpublishedmaps")]
        public async Task<ControllerResult<List<MapDto>>> GetPublishedMaps()
        {
            try
            {
                List<MapDto> mapList = await _mapService.GetPublishedMapsAsync();
                return ControllerResult<List<MapDto>>.Ok(mapList, "Pobrano listę opublikowanych map.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapsController.GetPublishedMaps");
                return ControllerResult<List<MapDto>>.ServerError("Wystąpił błąd podczas pobierania wszystkich opublikowanych map.");
            }
        }

        /// <summary>
        /// Pobiera 50 najpopularniejszych tagów.
        /// </summary>
        /// <returns>Lista nazw tagów</returns>
        [HttpGet("getpopulartagsnames")]
        public async Task<ControllerResult<List<string>>> GetPopularTagsNames()
        {
            try
            {
                List<string> tagsNames = await _tagService.GetPopularTagsAsync();
                return ControllerResult<List<string>>.Ok(tagsNames, "Pobrano listę popularnych tagów.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapsController.GetPopularTagsNames");
                return ControllerResult<List<string>>.ServerError("Wystąpił błąd podczas pobierania tagów.");
            }
        }

        /// <summary>
        /// Pobiera przefiltrowane mapy na podstawie podanych filtrów.
        /// </summary>
        /// <param name="SearchFilters">Hasła wyszukiwania</param>
        /// <param name="ChosenCountry">Nazwa kraju</param>
        /// <param name="QuantityPlaces">Ilość miejsc na mapie</param>
        /// <returns>Lista przefiltrowanych map</returns>
        [HttpGet("getfilteredmaps")]
        public async Task<ControllerResult<List<MapDto>>> GetFilteredMaps(
            [FromQuery] string Page,
            [FromQuery] List<string>? SearchFilters = null,
            [FromQuery] string? ChosenCountry = null,
            [FromQuery] string? QuantityPlaces = null)
        {
            try
            {
                List<MapDto> filteredMaps = await _mapService.GetFilteredMapsAsync(Page, SearchFilters, ChosenCountry, QuantityPlaces);
                return ControllerResult<List<MapDto>>.Ok(filteredMaps, "Pobrano listę przefiltrowanych map.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapsController.GetFilteredMaps");
                return ControllerResult<List<MapDto>>.ServerError("Wystąpił błąd podczas filtrowania map.");
            }
        }

        /// <summary>
        /// Pobiera wszystkie nazwy krajów.
        /// </summary>
        /// <returns>Lista nazw krajów</returns>
        [HttpGet("getcountrynames")]
        public async Task<ControllerResult<List<string>>> GetCountryNames()
        {
            try
            {
                List<string> countryNames = await _countryService.GetAllCountriesNamesAsync();
                return ControllerResult<List<string>>.Ok(countryNames, "Pobrano listę nazw krajów.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w MapsController.GetCountryNames");
                return ControllerResult<List<string>>.ServerError("Wystąpił błąd podczas pobierania wszystkich nazw krajów.");
            }
        }

    }
}
