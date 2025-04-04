using JurMaps.Model;
using JurMaps.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JurMaps.Repository
{
    /// <summary>
    /// Repozytorium odpowiedzialne za operacje na obiekcie Place w bazie danych.
    /// </summary>
    public class PlaceRepository : IPlaceRepository
    {
        private readonly MyDbContext _context;

        /// <summary>
        /// Inicjalizuje nowe repozytorium miejsc z kontekstem bazy danych.
        /// </summary>
        /// <param name="context">Kontekst bazy danych.</param>
        public PlaceRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Usuwa listę obiektów miejsc z bazy danych.
        /// </summary>
        /// <param name="places">Lista obiektów miejsc do usunięcia.</param>
        public async Task RemoveRangeAsync(List<Place> places)
        {
            _context.Places.RemoveRange(places);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Usuwa pojedynczy obiekt miejsca z bazy danych.
        /// </summary>
        /// <param name="place">Obiekt miejsca do usunięcia.</param>
        public async Task RemoveAsync(Place place)
        {
            _context.Places.Remove(place);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Dodaje nowy obiekt miejsca do bazy danych.
        /// </summary>
        /// <param name="place">Obiekt miejsca do dodania.</param>
        public async Task AddAsync(Place place)
        {
            _context.Places.Add(place);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera obiekt miejsca na podstawie identyfikatora.
        /// </summary>
        /// <param name="placeId">Identyfikator miejsca.</param>
        /// <returns>Obiekt miejsca lub null, jeśli nie znaleziono.</returns>
        public async Task<Place?> GetPlaceByIdAsync(int placeId)
        {
            return await _context.Places.FirstOrDefaultAsync(v => v.PlaceId == placeId);
        }

        /// <summary>
        /// Aktualizuje istniejący obiekt miejsca w bazie danych.
        /// </summary>
        /// <param name="place">Obiekt miejsca do zaktualizowania.</param>
        public async Task UpdateAsync(Place place)
        {
            _context.Update(place);
            await _context.SaveChangesAsync();
        }
    }

}
