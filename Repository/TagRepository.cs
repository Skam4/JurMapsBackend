using JurMaps.Model;
using JurMaps.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JurMaps.Repository
{
    /// <summary>
    /// Repozytorium odpowiedzialne za operacje na obiekcie Tag w bazie danych.
    /// </summary>
    public class TagRepository : ITagRepository
    {
        private readonly MyDbContext _context;

        /// <summary>
        /// Inicjalizuje nowe repozytorium tagów z kontekstem bazy danych.
        /// </summary>
        /// <param name="context">Kontekst bazy danych.</param>
        public TagRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Pobiera tag wraz z przypisanymi mapami na podstawie nazwy tagu.
        /// </summary>
        /// <param name="tagName">Nazwa tagu.</param>
        /// <returns>Obiekt tagu lub null, jeśli nie znaleziono.</returns>
        public async Task<Tag?> GetTagWithMapByTagNameAsync(string tagName)
        {
            return await _context.Tags.Include(t => t.MapsWithThisTag)
                       .FirstOrDefaultAsync(v => v.TagName == tagName);
        }

        /// <summary>
        /// Usuwa tag z bazy danych.
        /// </summary>
        /// <param name="tag">Obiekt tagu do usunięcia.</param>
        public async Task RemoveAsync(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Aktualizuje istniejący tag w bazie danych.
        /// </summary>
        /// <param name="tag">Obiekt tagu do zaktualizowania.</param>
        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Dodaje nowy tag do bazy danych.
        /// </summary>
        /// <param name="tag">Obiekt tagu do dodania.</param>
        public async Task AddAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera 50 tagów o największej liczbie wystąpień.
        /// </summary>
        /// <returns>Lista 50 najczęściej używanych tagów.</returns>
        public async Task<List<Tag>> Get50TagsByQuantityAsync()
        {
            return await _context.Tags.OrderByDescending(v => v.TagQuantity).Take(50).ToListAsync();
        }

        /// <summary>
        /// Pobiera tag po nazwie
        /// </summary>
        /// <param name="tagName">Nazwa tagu</param>
        /// <returns>Obiekt znalezionego tagu</returns>
        public async Task<Tag> GetTagsByNamesAsync(string tagName)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
        }
    }

}
