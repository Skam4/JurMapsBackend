using JurMaps.Model;
using Microsoft.EntityFrameworkCore;
using System;

/// <summary>
/// Repozytorium odpowiedzialne za operacje na obiekcie Country w bazie danych.
/// </summary>
public class CountryRepository : ICountryRepository
{
    private readonly MyDbContext _context;

    public CountryRepository(MyDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Pobiera kraj po jego nazwie
    /// </summary>
    /// <param name="countryName">Nazwa kraju</param>
    /// <returns>Znaleziony kraj</returns>
    public async Task<Country?> GetByNameAsync(string countryName)
    {
        return await _context.Countries.FirstOrDefaultAsync(c => c.CountryName == countryName);
    }

    /// <summary>
    /// Dodaje kraj do bazy
    /// </summary>
    /// <param name="country">Obiekt nowego kraju</param>
    /// <returns></returns>
    public async Task AddAsync(Country country)
    {
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Zwraca nazwy wszystkich krajów z bazy
    /// </summary>
    /// <returns>Lista nazw krajów</returns>
    public async Task<List<string>> GetAllCountryNamesAsync()
    {
        return await _context.Countries
            .Select(c => c.CountryName)
            .ToListAsync();
    }

    /// <summary>
    /// Aktualizuje obiekt kraju
    /// </summary>
    /// <param name="country">obiekt kraju</param>
    /// <returns></returns>
    public async Task UpdateAsync(Country country)
    {
        _context.Update(country);
        await _context.SaveChangesAsync();
    }
}
