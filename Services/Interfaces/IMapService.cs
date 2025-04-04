using JurMaps.Model;
using JurMaps.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace JurMaps.Services.Interfaces
{
    public interface IMapService
    {

        /// <summary>
        /// Zapisz mapę i zwróć jego id nowej mapy
        /// </summary>
        /// <param name="userId">ID użytkownika tworzącego mapę</param>
        /// <returns>Identyfikator mapy</returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task<int> SaveMapAndReturnIdAsync(int userId);

        /// <summary>
        /// Pobierz markery mapy
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <returns>Lista markerów mapy</returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task<List<MarkerDto>> GetMapMarkers(int mapId);
        Task<List<CircleDto>> GetMapCircles(int mapId);
        //Task<List<RectangleDto>> GetMapRectangles(int mapId);
        /// <summary>
        /// Pobiera wszystkie mapy i zamienia je w obiekt MapDto
        /// </summary>
        /// <returns>Lista obiektów MapDto</returns>
        Task<List<MapDto>> GetMapsAsync();
        /// <summary>
        /// Pobiera wszystkie opublikowane mapy i zamienia je w obiekt MapDto
        /// </summary>
        /// <returns>Lista obiektów MapDto</returns>
        Task<List<MapDto>> GetPublishedMapsAsync();

        /// <summary>
        /// Metoda zapisująca informację o mapie do bazy
        /// </summary>
        /// <param name="mapDto">Obiekt zapisywanej mapy</param>
        /// <param name="imageFile">Nowy plik zdjęcia mapy</param>
        Task SaveMapAsync(MapDto mapDto, IFormFile imageFile);

        /// <summary>
        /// Pobiera obiekt mapDto po id mapy
        /// </summary>
        /// <param name="id">id mapy</param>
        /// <returns>Zwraca mapDto</returns>
        /// <exception cref="ArgumentNullException">Gdy nie znaleziono mapy z podanym id</exception>
        Task<MapDto> GetMapDtoByIdAsync(int id);

        /// <summary>
        /// Wyszukuje mapę po id mapy
        /// </summary>
        /// <param name="id">id mapy</param>
        /// <returns>Zwraca obiekt mapy</returns>
        /// <exception cref="ArgumentNullException">Jeżeli mapa o podanym id nie została znaleziona</exception>
        Task<Map> GetMapByIdAsync(int id);

        /// <summary>
        /// Metoda filtrująca mapy po hasłach, kraju i ilości miejsc na mapie
        /// </summary>
        ///  <param name="Page">z której strony pobrać dane</param>
        /// <param name="SearchFilters">Hasła</param>
        /// <param name="ChosenCountry">Nazwa kraju</param>
        /// <param name="QuantityPlaces">Ilość miejsc na mapie</param>
        /// <returns></returns>
        Task<List<MapDto>> GetFilteredMapsAsync(string Page, List<string>? SearchFilters, string ChosenCountry = null, string QuantityPlaces = null);

        /// <summary>
        /// Pobiera wyznaczone opublikowane mapy użytkownika
        /// </summary>
        /// <param name="id">Id użytkownika</param>
        /// <param name="page">numer strony do pobrania</param>
        /// <returns>Listę map</returns>
        Task<List<MapDto>> GetPublicatedMapsByUserIdAsyncAndPageNumber(string id, string page);

        /// <summary>
        /// Pobiera wyznaczone nieopublikowane mapy użytkownika
        /// </summary>
        /// <param name="id">Id użytkownika</param>
        /// <param name="page">numer strony do pobrania</param>
        /// <returns>Listę map</returns>
        Task<List<MapDto>> GetPrivateMapsByUserIdAsyncAndPageNumber(string id, string page);

        /// <summary>
        /// Metoda aktualizująca mapę w bazie
        /// </summary>
        /// <param name="map">Mapa</param>
        Task UpdateMapAsync(Map map);

        /// <summary>
        /// Pobiera nieopublikowane mapy użytkownika
        /// </summary>
        /// <param name="id">id użytkownika</param>
        /// <returns>Lista nieopublikowanych map dto</returns>
        Task<List<MapDto>> GetPrivateMapsByUserIdAsync(string id);

        /// <summary>
        /// Usuwa mapę oraz wszystkie powiązane dane na podstawie jej ID
        /// </summary>
        /// <param name="mapId">ID mapy do usunięcia</param>
        /// <returns></returns>
        Task DeleteMapByIdAsync(int mapId);

        /// <summary>
        /// Publikuje mapę o podanym id
        /// </summary>
        /// <param name="mapId">id mapy</param>
        Task PublishMapAsync(int mapId);

        /// <summary>
        /// Przenosi mapę do wersji roboczych
        /// </summary>
        /// <param name="mapId"></param>
        Task MoveMapToDraftAsync(int mapId);

        /// <summary>
        /// Zwiększa liczbę polubień mapy
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task IncreaseLikesAsync(int mapId);

        /// <summary>
        /// Zmniejsza liczbę polubień mapy
        /// </summary>
        /// <param name="mapId">ID mapy.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task DecreaseLikesAsync(int mapId);
    }
}
