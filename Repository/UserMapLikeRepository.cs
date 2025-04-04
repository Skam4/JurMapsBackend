using JurMaps.Model;
using JurMaps.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JurMaps.Repository
{
    /// <summary>
    /// Repozytorium odpowiedzialne za operacje na obiekcie UserMapLike w bazie danych.
    /// </summary>
    public class UserMapLikeRepository : IUserMapLikeRepository
    {
        private readonly MyDbContext _context;

        /// <summary>
        /// Inicjalizuje nowe repozytorium polubień map przez użytkowników z kontekstem bazy danych.
        /// </summary>
        /// <param name="context">Kontekst bazy danych.</param>
        public UserMapLikeRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Usuwa listę obiektów polubień map przez użytkowników z bazy danych.
        /// </summary>
        /// <param name="userMapLikes">Lista obiektów polubień do usunięcia.</param>
        public async Task RemoveRangeAsync(List<UserMapLike> userMapLikes)
        {
            _context.UserMapLikes.RemoveRange(userMapLikes);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Usuwa pojedynczy obiekt polubienia mapy przez użytkownika.
        /// </summary>
        /// <param name="userMapLikes">Obiekt polubienia do usunięcia.</param>
        public async Task RemoveAsync(UserMapLike userMapLikes)
        {
            _context.UserMapLikes.Remove(userMapLikes);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera polubienie mapy przez użytkownika na podstawie identyfikatora użytkownika i mapy.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt polubienia mapy lub null, jeśli nie znaleziono.</returns>
        public async Task<UserMapLike?> GetUserMapLikeByUserIdAndMapId(int userId, int mapId)
        {
            return await _context.UserMapLikes
            .AsNoTracking()
            .FirstOrDefaultAsync(um => um.UserId == userId && um.MapId == mapId);

        }

        /// <summary>
        /// Dodaje nowe polubienie mapy przez użytkownika do bazy danych.
        /// </summary>
        /// <param name="userMapLike">Obiekt polubienia mapy do dodania.</param>
        public async Task AddAsync(UserMapLike userMapLike)
        {
            _context.UserMapLikes.Add(userMapLike);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera polubione mapy przez użytkownika z paginacją.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <param name="pageNumber">Numer strony.</param>
        /// <param name="pageSize">Liczba elementów na stronę.</param>
        /// <returns>Lista polubionych map użytkownika.</returns>
        public async Task<List<UserMapLike>> GetLikedMapsByUserIdAndPageNumberAsync(int userId, int pageNumber, int pageSize)
        {
            return await _context.UserMapLikes
                .Where(uml => uml.UserId == userId && uml.Map.MapUploaded == true)
                .Include(uml => uml.Map)
                .OrderBy(m => m.Map.MapId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }

}
