using JurMaps.Model;
using JurMaps.Model.DTO;

namespace JurMaps.Services.Interfaces
{
    public interface IUserMapLikesService
    {
        /// <summary>
        /// Metoda wyszukujący czy użytkownik polubił już mapę
        /// </summary>
        /// <param name="mapId">Id mapy</param>
        /// <param name="userId">Id użytkownika</param>
        /// <returns>Znalezione połączenie</returns>
        Task<UserMapLike?> WasMapLikedByUserAsync(int mapId, int userId);

        /// <summary>
        /// Dodaje nowy obiekt userMapLike do bazy
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="userId">ID użytkownika</param>
        /// <returns></returns>
        Task AddLikeAsync(int mapId, int userId);

        /// <summary>
        /// Usuwa obiekt userMapLike z bazy, usuwając polubienie mapy przez użytkownika
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="userId">ID użytkownika</param>
        /// <returns></returns>
        Task RemoveLikeAsync(int mapId, int userId);

        /// <summary>
        /// Pobiera wyznaczone polubione mapy użytkownika
        /// </summary>
        /// <param name="id">id użytkownika</param>
        /// <param name="page">numer strony do pobrania</param>
        /// <returns>lista map</returns>
        Task<List<MapDto>> GetLikedMapsByUserIdAndPageNumberAsync(string id, string page);
    }
}
