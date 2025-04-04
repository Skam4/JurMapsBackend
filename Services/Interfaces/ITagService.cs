using JurMaps.Model;

namespace JurMaps.Services.Interfaces
{
    public interface ITagService
    {
        /// <summary>
        /// Usuwa stare tagi danej mapy
        /// </summary>
        /// <param name="tagsNames">Lista nazw tagów</param>
        /// <param name="map">Mapa na której były tagi</param>
        /// <returns></returns>
        Task DeleteOldTagsAsync(List<string> tagsNames, Map map);

        /// <summary>
        /// Metoda pobierająca tagi po nazwach tagów
        /// </summary>
        /// <param name="tagsNames">Lista z nazwami tagów</param>
        /// <param name="map">Mapa do której należą tagi</param>
        /// <returns>Lista tagów</returns>
        Task<List<Tag>> GetTagsFromTagsNamesAsync(List<string> tagsNames, Map map);

        /// <summary>
        /// Metoda pobierająca 50 najpopularniejszych tagów
        /// </summary>
        /// <returns>Lista nazw tagów</returns>
        Task<List<string>> GetPopularTagsAsync();
    }
}
