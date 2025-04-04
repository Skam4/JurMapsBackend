using JurMaps.Model;
using JurMaps.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JurMaps.Repository
{
    /// <summary>
    /// Repozytorium odpowiedzialne za operacje na obiekcie Map w bazie danych.
    /// </summary>
    public class MapRepository : IMapRepository
    {
        private readonly MyDbContext _context;

        /// <summary>
        /// Inicjalizuje nowe repozytorium map z kontekstem bazy danych.
        /// </summary>
        /// <param name="context">Kontekst bazy danych.</param>
        public MapRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Dodaje nowy obiekt mapy do bazy danych i zwraca jego identyfikator.
        /// </summary>
        /// <param name="map">Obiekt mapy do dodania.</param>
        /// <returns>Identyfikator nowo dodanego obiektu mapy.</returns>
        public async Task<int> AddAsync(Map map)
        {
            _context.Maps.Add(map);
            await _context.SaveChangesAsync();
            return map.MapId;
        }

        /// <summary>
        /// Pobiera obiekt mapy wraz z powiązanymi miejscami na podstawie identyfikatora.
        /// </summary>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt mapy lub null, jeśli nie znaleziono.</returns>
        public async Task<Map?> GetByIdWithPlacesAsync(int mapId)
        {
            return await _context.Maps.Include(m => m.MapPlaces).Include(m => m.MapCountries).FirstOrDefaultAsync(m => m.MapId == mapId);
        }

        /// <summary>
        /// Pobiera wszystkie obiekty map z bazy danych.
        /// </summary>
        /// <returns>Lista obiektów map.</returns>
        public async Task<List<Map>> GetAllAsync()
        {
            return await _context.Maps.ToListAsync();
        }

        /// <summary>
        /// Pobiera obiekt mapy wraz ze wszystkimi powiązanymi szczegółami.
        /// </summary>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt mapy lub null, jeśli nie znaleziono.</returns>
        public async Task<Map?> GetByIdWithDetailsAsync(int mapId)
        {
            return await _context.Maps
                .Include(m => m.MapPlaces)
                .Include(m => m.MapTags)
                .Include(m => m.MapCountries)
                .Include(m => m.UserLikes)
                .FirstOrDefaultAsync(m => m.MapId == mapId);
        }

        /// <summary>
        /// Usuwa obiekt mapy z bazy danych.
        /// </summary>
        /// <param name="map">Obiekt mapy do usunięcia.</param>
        public async Task DeleteAsync(Map map)
        {
            _context.Maps.Remove(map);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera obiekt mapy na podstawie identyfikatora.
        /// </summary>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt mapy lub null, jeśli nie znaleziono.</returns>
        public async Task<Map?> GetByIdAsync(int mapId)
        {
            return await _context.Maps.Include(m => m.MapTags).FirstOrDefaultAsync(m => m.MapId == mapId);
        }

        /// <summary>
        /// Aktualizuje istniejący obiekt mapy w bazie danych.
        /// </summary>
        /// <param name="map">Obiekt mapy do zaktualizowania.</param>
        public async Task UpdateAsync(Map map)
        {
            _context.Maps.Update(map);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera opublikowane obiekty map wraz z ich tagami.
        /// </summary>
        /// <returns>Lista opublikowanych obiektów map.</returns>
        public async Task<List<Map>> GetPublicatedMapsWithTags()
        {
            return await _context.Maps.Include(t => t.MapTags).Where(v => v.MapUploaded == true).ToListAsync();
        }

        /// <summary>
        /// Pobiera wszystkie obiekty map z powiązanymi krajami.
        /// </summary>
        /// <returns>Zapytanie zwracające obiekty map z krajami.</returns>
        public IQueryable<Map> GetAllMapsWithCountries()
        {
            return _context.Maps
                .Include(m => m.MapCountries)
                .ThenInclude(mc => mc.Country)
                .Include(m => m.MapCreator);
        }

        /// <summary>
        /// Pobiera obiekty map użytkownika z paginacją.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="pageNumber">Numer strony.</param>
        /// <param name="pageSize">Liczba elementów na stronę.</param>
        /// <param name="ifUploaded">Określa, czy mapa jest opublikowana.</param>
        /// <returns>Lista obiektów map należących do użytkownika.</returns>
        public async Task<List<Map>> GetMapsByUserWithPaginationAsync(int userId, int pageNumber, int pageSize, bool ifUploaded)
        {
            return await _context.Maps
                .Where(m => m.MapCreatorId == userId && m.MapUploaded == ifUploaded)
                .OrderBy(m => m.MapId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Pobiera wszystkie prywatne obiekty map użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Lista prywatnych obiektów map użytkownika.</returns>
        public async Task<List<Map>> GetAllUserPrivateMaps(int userId)
        {
            return await _context.Maps.Where(v => v.MapCreatorId == userId).Where(v => v.MapUploaded == false).ToListAsync();
        }
    }

}
