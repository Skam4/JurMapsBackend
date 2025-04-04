using JurMaps.Model;
using JurMaps.Repository.Interfaces;
using JurMaps.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JurMaps.Services
{
    public class TagService : ITagService
    {
        private readonly IMapRepository _mapRepository;
        private readonly ITagRepository _tagRepository;
        public TagService(
            IMapRepository mapRepository,
            ITagRepository tagRepository
            )
        {
            _mapRepository = mapRepository;
            _tagRepository = tagRepository;
        }

        /// <summary>
        /// Usuwa stare tagi danej mapy
        /// </summary>
        /// <param name="tagsNames">Lista nazw tagów</param>
        /// <param name="map">Mapa na której były tagi</param>
        /// <returns></returns>
        public async Task DeleteOldTagsAsync(List<string> tagsNames, Map map)
        {
            if (tagsNames == null)
                tagsNames = new List<string>();

            foreach (string tagname in tagsNames)
            {
                var tag = await _tagRepository.GetTagWithMapByTagNameAsync(tagname);

                if (tag != null)
                {
                    if (tag.MapsWithThisTag.Contains(map))
                    {
                        tag.MapsWithThisTag.Remove(map);
                        map.MapTags.Remove(tag);
                    }
                    if (tag.TagQuantity > 0)
                    {
                        tag.TagQuantity--;
                    }
                    if (tag.TagQuantity <= 0)
                    {
                        await _tagRepository.RemoveAsync(tag);
                    }
                }
            }
        }

        /// <summary>
        /// Metoda pobierająca tagi po nazwach tagów
        /// </summary>
        /// <param name="tagsNames">Lista z nazwami tagów</param>
        /// <param name="map">Mapa do której należą tagi</param>
        /// <returns>Lista tagów</returns>
        public async Task<List<Tag>> GetTagsFromTagsNamesAsync(List<string> tagsNames, Map map)
        {
            if (tagsNames == null || !tagsNames.Any())
                return new List<Tag>();

            var tagList = new List<Tag>();

            foreach (var tagName in tagsNames)
            {
                var tag = await _tagRepository.GetTagsByNamesAsync(tagName);
                if (tag != null)
                {
                    if (!tag.MapsWithThisTag.Any(m => m.MapId == map.MapId))
                    {
                        tag.MapsWithThisTag.Add(map);
                        tag.TagQuantity++;
                        await _tagRepository.UpdateAsync(tag);
                    }
                }
                else
                {
                    tag = new Tag { TagName = tagName, MapsWithThisTag = new List<Map> { map }, TagQuantity = 1 };
                    await _tagRepository.AddAsync(tag);
                }
                tagList.Add(tag);
            }

            return tagList;
        }


        /// <summary>
        /// Metoda pobierająca 50 najpopularniejszych tagów
        /// </summary>
        /// <returns>Lista nazw tagów</returns>
        public async Task<List<string>> GetPopularTagsAsync()
        {
            List<Tag> popularTags = await _tagRepository.Get50TagsByQuantityAsync();
            List<string> tagsNames = popularTags.Select(tag => tag.TagName).ToList();

            return tagsNames;
        }
    }
}
