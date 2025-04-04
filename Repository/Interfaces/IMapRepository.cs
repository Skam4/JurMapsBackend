using JurMaps.Model;

namespace JurMaps.Repository.Interfaces
{
    public interface IMapRepository
    {
        /// <summary>
        /// Dodaje nowy obiekt mapy do bazy danych i zwraca jego identyfikator.
        /// </summary>
        /// <param name="map">Obiekt mapy do dodania.</param>
        /// <returns>Identyfikator nowo dodanego obiektu mapy.</returns>
        Task<int> AddAsync(Map map);

        /// <summary>
        /// Pobiera obiekt mapy wraz z powiązanymi miejscami na podstawie identyfikatora.
        /// </summary>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt mapy lub null, jeśli nie znaleziono.</returns>
        Task<Map?> GetByIdWithPlacesAsync(int mapId);

        /// <summary>
        /// Pobiera wszystkie obiekty map z bazy danych.
        /// </summary>
        /// <returns>Lista obiektów map.</returns>
        Task<List<Map>> GetAllAsync();

        /// <summary>
        /// Pobiera obiekt mapy wraz ze wszystkimi powiązanymi szczegółami.
        /// </summary>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt mapy lub null, jeśli nie znaleziono.</returns>
        Task<Map?> GetByIdWithDetailsAsync(int mapId);

        /// <summary>
        /// Usuwa obiekt mapy z bazy danych.
        /// </summary>
        /// <param name="map">Obiekt mapy do usunięcia.</param>
        Task DeleteAsync(Map map);

        /// <summary>
        /// Pobiera obiekt mapy na podstawie identyfikatora.
        /// </summary>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt mapy lub null, jeśli nie znaleziono.</returns>
        Task<Map?> GetByIdAsync(int mapId);

        /// <summary>
        /// Aktualizuje istniejący obiekt mapy w bazie danych.
        /// </summary>
        /// <param name="map">Obiekt mapy do zaktualizowania.</param>
        Task UpdateAsync(Map map);

        /// <summary>
        /// Pobiera opublikowane obiekty map wraz z ich tagami.
        /// </summary>
        /// <returns>Lista opublikowanych obiektów map.</returns>
        Task<List<Map>> GetPublicatedMapsWithTags();

        /// <summary>
        /// Pobiera wszystkie obiekty map z powiązanymi krajami.
        /// </summary>
        /// <returns>Zapytanie zwracające obiekty map z krajami.</returns>
        IQueryable<Map> GetAllMapsWithCountries();

        /// <summary>
        /// Pobiera obiekty map użytkownika z paginacją.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="pageNumber">Numer strony.</param>
        /// <param name="pageSize">Liczba elementów na stronę.</param>
        /// <param name="ifUploaded">Określa, czy mapa jest opublikowana.</param>
        /// <returns>Lista obiektów map należących do użytkownika.</returns>
        Task<List<Map>> GetMapsByUserWithPaginationAsync(int userId, int pageNumber, int pageSize, bool ifUploaded);

        /// <summary>
        /// Pobiera wszystkie prywatne obiekty map użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Lista prywatnych obiektów map użytkownika.</returns>
        Task<List<Map>> GetAllUserPrivateMaps(int userId);
    }
}
