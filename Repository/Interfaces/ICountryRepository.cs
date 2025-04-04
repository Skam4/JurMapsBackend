using JurMaps.Model;

public interface ICountryRepository
{
    /// <summary>
    /// Pobiera kraj po jego nazwie
    /// </summary>
    /// <param name="countryName">Nazwa kraju</param>
    /// <returns>Znaleziony kraj</returns>
    Task<Country?> GetByNameAsync(string countryName);

    /// <summary>
    /// Dodaje kraj do bazy
    /// </summary>
    /// <param name="country">Obiekt nowego kraju</param>
    /// <returns></returns>
    Task AddAsync(Country country);

    /// <summary>
    /// Zwraca nazwy wszystkich krajów z bazy
    /// </summary>
    /// <returns>Lista nazw krajów</returns>
    Task<List<string>> GetAllCountryNamesAsync();

    /// <summary>
    /// Aktualizuje obiekt kraju
    /// </summary>
    /// <param name="country">obiekt kraju</param>
    /// <returns></returns>
    Task UpdateAsync(Country country);
}
