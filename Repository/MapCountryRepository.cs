using JurMaps.Model;
using JurMaps.Repository.Interfaces;

namespace JurMaps.Repository
{
    /// <summary>
    /// Repozytorium odpowiedzialne za operacje na obiekcie MapCountry w bazie danych.
    /// </summary>
    public class MapCountryRepository : IMapCountryRepository
    {
        private readonly MyDbContext _context;

        /// <summary>
        /// Inicjalizuje nowe repozytorium z kontekstem bazy danych.
        /// </summary>
        /// <param name="context">Kontekst bazy danych.</param>
        public MapCountryRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Usuwa listę obiektów MapCountry z bazy danych.
        /// </summary>
        /// <param name="mapCountries">Lista obiektów do usunięcia.</param>
        public async Task RemoveRangeAsync(List<MapCountry> mapCountries)
        {
            _context.MapCountries.RemoveRange(mapCountries);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera obiekt MapCountry na podstawie identyfikatorów kraju i mapy.
        /// </summary>
        /// <param name="countryId">Identyfikator kraju.</param>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt MapCountry lub null, jeśli nie znaleziono.</returns>
        public MapCountry? GetByMapAndCountryById(int countryId, int mapId)
        {
            return _context.MapCountries.FirstOrDefault(v => v.CountryId == countryId && v.MapId == mapId);
        }

        /// <summary>
        /// Aktualizuje istniejący obiekt MapCountry w bazie danych.
        /// </summary>
        /// <param name="mapCountry">Obiekt do zaktualizowania.</param>
        public async Task UpdateAsync(MapCountry mapCountry)
        {
            _context.Update(mapCountry);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Usuwa obiekt MapCountry z bazy danych.
        /// </summary>
        /// <param name="mapCountry">Obiekt do usunięcia.</param>
        public async Task RemoveAsync(MapCountry mapCountry)
        {
            _context.Remove(mapCountry);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Dodaje nowy obiekt MapCountry do bazy danych.
        /// </summary>
        /// <param name="mapCountry">Obiekt do dodania.</param>
        public async Task AddAsync(MapCountry mapCountry)
        {
            _context.Add(mapCountry);
            await _context.SaveChangesAsync();
        }
    }
}
