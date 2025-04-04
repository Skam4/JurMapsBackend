namespace JurMaps.Services.Interfaces
{
    public interface ICountryService
    {
        /// <summary>
        /// Metoda pobierająca nazwy wszystkich krajów z bazy
        /// </summary>
        /// <returns>lista nazw krajów</returns>
        Task<List<string>> GetAllCountriesNamesAsync();

        /// <summary>
        /// Metoda zapisująca kraj do danej mapy
        /// </summary>
        /// <param name="map">mapa</param>
        /// <param name="countryName">nazwa kraju</param>
        Task AddMapCountryAsync(int mapId, string countryName);
    }
}
