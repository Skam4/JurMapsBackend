using JurMaps.Model;

namespace JurMaps.Repository.Interfaces
{
    public interface ITagRepository
    {
        /// <summary>
        /// Pobiera tag wraz z przypisanymi mapami na podstawie nazwy tagu.
        /// </summary>
        /// <param name="tagName">Nazwa tagu.</param>
        /// <returns>Obiekt tagu lub null, jeśli nie znaleziono.</returns>
        Task<Tag?> GetTagWithMapByTagNameAsync(string tagName);

        /// <summary>
        /// Usuwa tag z bazy danych.
        /// </summary>
        /// <param name="tag">Obiekt tagu do usunięcia.</param>
        Task RemoveAsync(Tag tag);

        /// <summary>
        /// Aktualizuje istniejący tag w bazie danych.
        /// </summary>
        /// <param name="tag">Obiekt tagu do zaktualizowania.</param>
        Task UpdateAsync(Tag tag);

        /// <summary>
        /// Dodaje nowy tag do bazy danych.
        /// </summary>
        /// <param name="tag">Obiekt tagu do dodania.</param>
        Task AddAsync(Tag tag);

        /// <summary>
        /// Pobiera 50 tagów o największej liczbie wystąpień.
        /// </summary>
        /// <returns>Lista 50 najczęściej używanych tagów.</returns>
        Task<List<Tag>> Get50TagsByQuantityAsync();

        /// <summary>
        /// Pobiera tag po nazwie
        /// </summary>
        /// <param name="tagName">Nazwa tagu</param>
        /// <returns>Obiekt znalezionego tagu</returns>
        Task<Tag> GetTagsByNamesAsync(string tagName);
    }
}
