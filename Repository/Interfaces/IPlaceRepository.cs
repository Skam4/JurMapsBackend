using JurMaps.Model;

namespace JurMaps.Repository.Interfaces
{
    public interface IPlaceRepository
    {
        /// <summary>
        /// Usuwa listę obiektów miejsc z bazy danych.
        /// </summary>
        /// <param name="places">Lista obiektów miejsc do usunięcia.</param>
        Task RemoveRangeAsync(List<Place> places);

        /// <summary>
        /// Usuwa pojedynczy obiekt miejsca z bazy danych.
        /// </summary>
        /// <param name="place">Obiekt miejsca do usunięcia.</param>
        Task RemoveAsync(Place place);

        /// <summary>
        /// Dodaje nowy obiekt miejsca do bazy danych.
        /// </summary>
        /// <param name="place">Obiekt miejsca do dodania.</param>
        Task AddAsync(Place place);

        /// <summary>
        /// Pobiera obiekt miejsca na podstawie identyfikatora.
        /// </summary>
        /// <param name="placeId">Identyfikator miejsca.</param>
        /// <returns>Obiekt miejsca lub null, jeśli nie znaleziono.</returns>
        Task<Place?> GetPlaceByIdAsync(int placeId);

        /// <summary>
        /// Aktualizuje istniejący obiekt miejsca w bazie danych.
        /// </summary>
        /// <param name="place">Obiekt miejsca do zaktualizowania.</param>
        Task UpdateAsync(Place place);

    }
}
