using JurMaps.Model;
using JurMaps.Model.DTO;
using NetTopologySuite.Geometries;

namespace JurMaps.Services.Interfaces
{
    public interface IPlaceService
    {
        /// <summary>
        /// Zapisywanie markera do bazy
        /// </summary>
        /// <param name="marker">Obiekt markera</param>
        /// <param name="mapId">id mapy</param>
        /// <returns>Identifikator markera</returns>
        Task<int> AddMarkerAsync(MarkerDto marker, int mapId);
        Task<int> AddCircleAsync(string name, string description, Point circle, double radius, int mapId);
        Task<int> AddRectangleAsync(string name, string description, List<Point> points, int mapId);

        /// <summary>
        /// Zapisywanie informacji o miejscu do bazy
        /// </summary>
        /// <param name="placeId">id miejsca</param>
        /// <param name="placeName">nazwa miejsca</param>
        /// <param name="placeDescription">opis miejsca</param>
        /// <param name="placePhoto1">zdjęcie pierwsze</param>
        /// <param name="placePhoto2">zdjęcie drugie</param>
        /// <param name="placePhoto3">zdjęcie trzecie</param>
        /// <param name="placeColor">kolor oznaczenia miejsca</param>
        /// /// <param name="changePlacePhoto1">Oznaczenie czy zdjęcie zostało zmienione</param>
        /// /// <param name="changePlacePhoto2">Oznaczenie czy zdjęcie zostało zmienione</param>
        /// /// <param name="changePlacePhoto3">Oznaczenie czy zdjęcie zostało zmienione</param>
        Task SavePlaceInfoAsync(
            int placeId,
            string placeName,
            string placeDescription,
            IFormFile? placePhoto1,
            IFormFile? placePhoto2,
            IFormFile? placePhoto3,
            string placeColor,
            bool changePlacePhoto1 = false,
            bool changePlacePhoto2 = false,
            bool changePlacePhoto3 = false);

        /// <summary>
        /// Pobiera obiekt miejsca po identyfikatorze
        /// </summary>
        /// <param name="id">ID miejsca</param>
        /// <returns>Znalezione miejsce</returns>
        Task<Place?> GetPlaceByIdAsync(int id);

        /// <summary>
        /// Aktualizuje położenie markera
        /// </summary>
        /// <param name="markerId">ID markera</param>
        /// <param name="position">Nowe położenie markera</param>
        /// <returns></returns>
        Task UpdateMarkerPositionAsync(int markerId, PositionDto position);

        /// <summary>
        /// Usuwa miejsce po id
        /// </summary>
        /// <param name="placeId">id miejsca</param>
        Task DeleteAsync(int placeId);
    }
}
