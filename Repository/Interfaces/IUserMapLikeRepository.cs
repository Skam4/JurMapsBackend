using JurMaps.Model;

namespace JurMaps.Repository.Interfaces
{
    public interface IUserMapLikeRepository
    {
        /// <summary>
        /// Usuwa listę obiektów polubień map przez użytkowników z bazy danych.
        /// </summary>
        /// <param name="userMapLikes">Lista obiektów polubień do usunięcia.</param>
        Task RemoveRangeAsync(List<UserMapLike> userMapLikes);

        /// <summary>
        /// Usuwa pojedynczy obiekt polubienia mapy przez użytkownika.
        /// </summary>
        /// <param name="userMapLikes">Obiekt polubienia do usunięcia.</param>
        Task RemoveAsync(UserMapLike userMapLikes);

        /// <summary>
        /// Pobiera polubienie mapy przez użytkownika na podstawie identyfikatora użytkownika i mapy.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt polubienia mapy lub null, jeśli nie znaleziono.</returns>
        Task<UserMapLike?> GetUserMapLikeByUserIdAndMapId(int userId, int mapId);

        /// <summary>
        /// Dodaje nowe polubienie mapy przez użytkownika do bazy danych.
        /// </summary>
        /// <param name="userMapLike">Obiekt polubienia mapy do dodania.</param>
        Task AddAsync(UserMapLike userMapLike);

        /// <summary>
        /// Pobiera polubione mapy przez użytkownika z paginacją.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="pageNumber">Numer strony.</param>
        /// <param name="pageSize">Liczba elementów na stronę.</param>
        /// <returns>Lista polubionych map użytkownika.</returns>
        Task<List<UserMapLike>> GetLikedMapsByUserIdAndPageNumberAsync(int userId, int pageNumber, int pageSize);
    }
}
