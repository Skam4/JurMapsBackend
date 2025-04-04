using JurMaps.Model;
using JurMaps.Repository;
using JurMaps.Repository.Interfaces;
using JurMaps.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JurMaps.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapRepository _mapRepository;
        private readonly IMapCountryRepository _mapCountryRepository;
        private readonly ILogger<CountryService> _logger;

        public CountryService(
            ICountryRepository countryRepository,
            IMapRepository mapRepository,
            IMapCountryRepository mapCountryRepository,
            ILogger<CountryService> logger
            )
        {
            _countryRepository = countryRepository;
            _mapRepository = mapRepository;
            _mapCountryRepository = mapCountryRepository;
            _logger = logger;
        }

        /// <summary>
        /// Metoda pobierająca nazwy wszystkich krajów z bazy.
        /// </summary>
        /// <returns>Lista nazw krajów</returns>
        public async Task<List<string>> GetAllCountriesNamesAsync()
        {
            try
            {
                return await _countryRepository.GetAllCountryNamesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wywoływania _countryRepository.GetAllCountryNamesAsync()");
                throw;
            }
        }

        /// <summary>
        /// Metoda zapisująca kraj do danej mapy
        /// </summary>
        /// <param name="mapId">mapa</param>
        /// <param name="countryName">nazwa kraju</param>
        public async Task AddMapCountryAsync(int mapId, string countryName)
        {
            var map = await _mapRepository.GetByIdAsync(mapId);
            if (map == null)
                throw new Exception("Nie znaleziono mapy z podanym identyfikatorem.");

            Country? country = await _countryRepository.GetByNameAsync(countryName);
            if (country == null)
            {
                country = new Country(countryName);
                await _countryRepository.AddAsync(country);
            }

            MapCountry? mapCountry = _mapCountryRepository.GetByMapAndCountryById(country.CountryId, mapId);

            if (mapCountry != null)
            {
                mapCountry.ConnectionCount++;
                await _mapCountryRepository.UpdateAsync(mapCountry);
            }
            else
            {
                mapCountry = new MapCountry
                {
                    MapId = mapId,
                    CountryId = country.CountryId
                };
                await _mapCountryRepository.AddAsync(mapCountry);
            }

            map.MapCountries.Add(mapCountry);
            country.MapCountries.Add(mapCountry);
            await _mapRepository.UpdateAsync(map);
            await _countryRepository.UpdateAsync(country);

        }
    }
}
