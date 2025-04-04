using JurMaps.Model;

namespace JurMaps.Repository.Interfaces
{
    public interface IMapCountryRepository
    {
        /// <summary>
        /// Usuwa listę obiektów MapCountry z bazy danych.
        /// </summary>
        /// <param name="mapCountries">Lista obiektów do usunięcia.</param>
        Task RemoveRangeAsync(List<MapCountry> mapCountries);

        /// <summary>
        /// Pobiera obiekt MapCountry na podstawie identyfikatorów kraju i mapy.
        /// </summary>
        /// <param name="countryId">Identyfikator kraju.</param>
        /// <param name="mapId">Identyfikator mapy.</param>
        /// <returns>Obiekt MapCountry lub null, jeśli nie znaleziono.</returns>
        MapCountry? GetByMapAndCountryById(int countryId, int mapId);

        /// <summary>
        /// Aktualizuje istniejący obiekt MapCountry w bazie danych.
        /// </summary>
        /// <param name="mapCountry">Obiekt do zaktualizowania.</param>
        Task UpdateAsync(MapCountry mapCountry);

        /// <summary>
        /// Usuwa obiekt MapCountry z bazy danych.
        /// </summary>
        /// <param name="mapCountry">Obiekt do usunięcia.</param>
        Task RemoveAsync(MapCountry mapCountry);

        /// <summary>
        /// Dodaje nowy obiekt MapCountry do bazy danych.
        /// </summary>
        /// <param name="mapCountry">Obiekt do dodania.</param>
        Task AddAsync(MapCountry mapCountry);

    }
}
