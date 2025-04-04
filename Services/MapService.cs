using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Repository;
using JurMaps.Repository.Interfaces;
using JurMaps.Services.Interfaces;
using JurMaps.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static NetTopologySuite.Geometries.Utilities.GeometryMapper;
using static System.Net.Mime.MediaTypeNames;

namespace JurMaps.Services
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IMapCountryRepository _mapCountryRepository;
        private readonly IUserMapLikeRepository _userMapLikeRepository;
        private readonly ITagService _tagService;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ILogger<MapService> _logger;
        private readonly Helpers _helpers;

        public MapService(
            IMapRepository mapRepository, 
            IUserRepository userRepository, 
            IFirebaseStorageService firebaseStorageService,
            IPlaceRepository placeRepository, 
            IMapCountryRepository mapCountryRepository,
            ITagService tagService,
            IUserMapLikeRepository userMapLikeRepository,
            ILogger<MapService> logger,
            Helpers helpers
            )
        {
            _mapRepository = mapRepository;
            _userRepository = userRepository;
            _placeRepository = placeRepository;
            _mapCountryRepository = mapCountryRepository;
            _tagService = tagService;
            _userMapLikeRepository = userMapLikeRepository;
            _firebaseStorageService = firebaseStorageService;
            _logger = logger;
            _helpers = helpers;
        }

        /// <summary>
        /// Zapisz mapę i zwróć jego id nowej mapy
        /// </summary>
        /// <param name="userId">ID użytkownika tworzącego mapę</param>
        /// <returns>Identyfikator mapy</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<int> SaveMapAndReturnIdAsync(int userId)
        {
            User? owner = await _userRepository.GetUserByIdAsync(userId);
            if (owner == null)
                throw new Exception("Nie znaleziono użytkownika.");

            int countMapsCreatedToday = 0;
            List<Map> userMaps = owner.UserMaps.ToList();
            foreach (Map map in userMaps) { 
                if(map == null) continue;
                if(map.MapCreationDate == DateTime.Now.ToString("dd.MM.yyyy")) { countMapsCreatedToday++; }
                if(countMapsCreatedToday >= 5)
                {
                    throw new Exception("Możesz stworzyć tylko 5 map na dzień. Wyczerpałeś limit.");
                }
            }

            Map newMap = new Map { 
                MapCreator = owner,
                MapCreationDate = DateTime.Now.ToString("dd.MM.yyyy")
            };

            if (newMap.MapCreator == null || newMap.MapCreator.UserId == 0)
                throw new InvalidOperationException("Mapa nie ma przypisanego użytkownika");

            _userRepository.Attach(newMap.MapCreator);
            return await _mapRepository.AddAsync(newMap);
        }

        /// <summary>
        /// Pobierz markery mapy
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <returns>Lista markerów mapy</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<List<MarkerDto>> GetMapMarkers(int mapId)
        {
            var map = await _mapRepository.GetByIdWithPlacesAsync(mapId);
            if (map == null)
                throw new InvalidOperationException("Mapa o podanym identyfikatorze nie istnieje.");

            var markers = map.MapPlaces.Where(v => v.PlaceType == "Marker").ToList();
            var markerDtos = new List<MarkerDto>();

            foreach (var marker in markers)
            {
                var markerDto = new MarkerDto
                {
                    MarkerId = marker.PlaceId,
                    Position = new PositionDto { X = marker.PlacePosition.X, Y = marker.PlacePosition.Y },
                    MarkerName = marker.PlaceName,
                    MarkerDescription = marker.PlaceDescription,
                    PlaceColor = marker.PlaceColor,
                    PlaceCountry = marker.PlaceCountry
                };

                // Jeśli pole nie jest puste, pobieramy URL z FirebaseStorageService
                if (!string.IsNullOrEmpty(marker.PlacePhoto1))
                {
                    markerDto.PlacePhoto1 = await _firebaseStorageService.GetSignedUrlAsync(marker.PlacePhoto1);
                }
                else
                {
                    markerDto.PlacePhoto1 = marker.PlacePhoto1;
                }

                if (!string.IsNullOrEmpty(marker.PlacePhoto2))
                {
                    markerDto.PlacePhoto2 = await _firebaseStorageService.GetSignedUrlAsync(marker.PlacePhoto2);
                }
                else
                {
                    markerDto.PlacePhoto2 = marker.PlacePhoto2;
                }

                if (!string.IsNullOrEmpty(marker.PlacePhoto3))
                {
                    markerDto.PlacePhoto3 = await _firebaseStorageService.GetSignedUrlAsync(marker.PlacePhoto3);
                }
                else
                {
                    markerDto.PlacePhoto3 = marker.PlacePhoto3;
                }

                markerDtos.Add(markerDto);
            }

            return markerDtos;
        }

        public async Task<List<CircleDto>> GetMapCircles(int mapId)
        {
            var map = await _mapRepository.GetByIdWithPlacesAsync(mapId);
            return map?.MapPlaces
                .Where(v => v.PlaceType == "Circle")
                .Select(circle => new CircleDto
                {
                    CircleId = circle.PlaceId,
                    Position = new PositionDto { X = circle.PlacePosition.X, Y = circle.PlacePosition.Y },
                    CircleName = circle.PlaceName,
                    CircleDescription = circle.PlaceDescription
                }).ToList() ?? new List<CircleDto>();
        }

        /*        public async Task<List<RectangleDto>> GetMapRectangles(int mapId)
                {
                    var rectangleList = _context.Maps.Include(m => m.MapPlaces).FirstOrDefault(v => v.MapId == mapId).MapPlaces.Where(v => v.PlaceType == "Rectangle").ToList();

                    List<RectangleDto> rectangleDtoList = new List<RectangleDto>();

                    foreach (var rectangle in rectangleList)
                    {
                        RectangleDto rectangleDto = new RectangleDto
                        {
                            RectangleId = rectangle.PlaceId,
                            Bounds = new List<PositionDto>()
                        };

        *//*                if (rectangle.Bounds != null)
                        {
                            var coordinates = rectangle.Bounds.Coordinates;

                            rectangleDto.Bounds.Add(new PositionDto
                            {
                                X = coordinates[0].X,
                                Y = coordinates[0].Y
                            });
                            rectangleDto.Bounds.Add(new PositionDto
                            {
                                X = coordinates[2].X,
                                Y = coordinates[2].Y
                            });

                        }*//*

                        rectangleDtoList.Add(rectangleDto);
                    }

                    return rectangleDtoList;
                }*/

        /// <summary>
        /// Pobiera wszystkie mapy i zamienia je w obiekt MapDto
        /// </summary>
        /// <returns>Lista obiektów MapDto</returns>
        public async Task<List<MapDto>> GetMapsAsync()
        {
            var maps = await _mapRepository.GetAllAsync();
            return maps.Select(map => new MapDto
            {
                MapId = map.MapId,
                MapName = map.MapName,
                MapDescription = map.MapDescription,
                MapPublicationDate = map.MapPublicationDate,
                MapLikes = map.MapLikes,
                MapUploaded = map.MapUploaded
            }).ToList();
        }

        /// <summary>
        /// Pobiera wszystkie opublikowane mapy i zamienia je w obiekt MapDto
        /// </summary>
        /// <returns>Lista obiektów MapDto</returns>
        public async Task<List<MapDto>> GetPublishedMapsAsync()
        {
            var mapList = await _mapRepository.GetPublicatedMapsWithTags();
            var mapDtoList = new List<MapDto>();

            foreach (var map in mapList)
            {
                MapDto mapDto = new MapDto
                {
                    MapId = map.MapId,
                    MapName = map.MapName,
                    MapDescription = map.MapDescription,
                    MapPublicationDate = map.MapPublicationDate,
                    MapThumbnail = map.MapThumbnail,
                    MapLikes = map.MapLikes,
                    MapTags = map.MapTags.Select(tag => tag.TagName).ToList(),
                    MapCreatorId = map.MapCreatorId,
                    MapCreatorName = map.MapCreator?.UserName,
                    MapCreatorPicture = await _firebaseStorageService.GetSignedUrlAsync(map.MapCreator?.UserProfilePicture)
                };

                mapDtoList.Add(mapDto);
            }

            return mapDtoList;
        }

        /// <summary>
        /// Metoda zapisująca informację o mapie do bazy
        /// </summary>
        /// <param name="mapDto">Obiekt zapisywanej mapy</param>
        /// <param name="imageFile">Nowy plik zdjęcia mapy</param>
        public async Task SaveMapAsync(MapDto mapDto, IFormFile imageFile)
        {
            if (mapDto == null)
                throw new ArgumentNullException(nameof(mapDto), "Problem z danymi mapy.");

            float toxicityScore = await _helpers.AnalyzeText(mapDto.MapName);

            if (toxicityScore >= 0.10)
            {
                throw new InvalidOperationException("Tytuł mapy jest zbyt toksyczny i nie może zostać zapisany. Zmień tytuł mapy.");
            }

            toxicityScore = await _helpers.AnalyzeText(mapDto.MapDescription);

            if (toxicityScore >= 0.10)
            {
                throw new InvalidOperationException("Opis mapy jest zbyt toksyczny i nie może zostać zapisany. Zmień opis mapy.");
            }

            await _helpers.CheckImageAsync(imageFile);

            Map map = await GetMapByIdAsync(mapDto.MapId);

            foreach (var tagName in mapDto.MapTags)
            {
                toxicityScore = await _helpers.AnalyzeText(tagName);

                if (toxicityScore >= 0.10)
                {
                    throw new InvalidOperationException("Słowa kluczowe mapy są zbyt toksyczne i należy je zmienić.");
                }
            }

            await _tagService.DeleteOldTagsAsync(mapDto.MapOldTags, map);
            List<Tag> tagList = await _tagService.GetTagsFromTagsNamesAsync(mapDto.MapTags, map);

            if (string.IsNullOrEmpty(mapDto.MapName) || string.IsNullOrEmpty(mapDto.MapDescription))
            {
                throw new ArgumentNullException("mapName/mapDesc", "Błędne dane przy zapisie mapy");
            }

            if (map.MapPlaces == null || !map.MapPlaces.Any())
            {
                map = await GetMapByIdAsync(map.MapId);
            }

            if (!string.IsNullOrEmpty(map.MapThumbnail) && map.MapThumbnail != mapDto.MapThumbnail)
            {
                await _firebaseStorageService.DeleteFileAsync(map.MapThumbnail);
            }

            map.MapName = mapDto.MapName;
            map.MapDescription = mapDto.MapDescription;
            map.MapUploaded = mapDto.MapUploaded;

            if (tagList.Count > 0)
            {
                map.MapTags = tagList;
            }

            if(imageFile != null)
                map.MapThumbnail = await _firebaseStorageService.UploadFileAsync(imageFile);

            if (mapDto.MapUploaded)
            {
                map.MapPublicationDate = DateTime.Now.ToString("dd.MM.yyyy");
            }

            await _mapRepository.UpdateAsync(map);
        }


        /// <summary>
        /// Pobiera obiekt mapDto po id mapy
        /// </summary>
        /// <param name="mapId">id mapy</param>
        /// <returns>Obiekt mapDto</returns>
        /// <exception cref="ArgumentNullException">Gdy nie znaleziono mapy z podanym id</exception>
        public async Task<MapDto> GetMapDtoByIdAsync(int mapId)
        {
            if (mapId <= 0)
                throw new ArgumentNullException("Błędne id.");

            var map = await _mapRepository.GetByIdAsync(mapId);
            if (map == null)
                throw new ArgumentNullException("Nie znaleziono mapy o podanym id");

            User? owner = await _userRepository.GetUserByIdAsync(map.MapCreatorId);

            var mapDto = new MapDto
            {
                MapId = map.MapId,
                MapName = map.MapName,
                MapDescription = map.MapDescription,
                MapUploaded = map.MapUploaded,
                MapTags = map.MapTags != null ? map.MapTags.Select(tag => tag.TagName).ToList() : new List<string>(),
                MapPublicationDate = map.MapPublicationDate,
                MapLikes = map.MapLikes,
                MapCreatorId = map.MapCreatorId,
                MapCreatorName = owner.UserName,
                MapCountries = map.MapCountries.Select(country => country.Country.CountryName).ToList(),
            };

            if(map.MapThumbnail != null)
                mapDto.MapThumbnail = await _firebaseStorageService.GetSignedUrlAsync(map.MapThumbnail);

            if(owner.UserProfilePicture != null)
                mapDto.MapCreatorPicture = await _firebaseStorageService.GetSignedUrlAsync(owner.UserProfilePicture);

            return mapDto;
        }


        /// <summary>
        /// Wyszukuje mapę po id mapy
        /// </summary>
        /// <param name="id">id mapy</param>
        /// <returns>Zwraca obiekt mapy</returns>
        /// <exception cref="ArgumentNullException">Jeżeli mapa o podanym id nie została znaleziona</exception>
        public async Task<Map> GetMapByIdAsync(int mapId)
        {
            Map? map = await _mapRepository.GetByIdAsync(mapId);
            if (map == null)
            {
                throw new ArgumentNullException("Map not found with given id");
            }

            return map;
        }

        /// <summary>
        /// Metoda filtrująca mapy po hasłach, kraju i ilości miejsc na mapie
        /// </summary>
        ///  <param name="Page">z której strony pobrać dane</param>
        /// <param name="SearchFilters">Hasła</param>
        /// <param name="ChosenCountry">Nazwa kraju</param>
        /// <param name="QuantityPlaces">Ilość miejsc na mapie</param>
        /// <returns></returns>
        public async Task<List<MapDto>> GetFilteredMapsAsync
            (string Page, List<string>? SearchFilters, string ChosenCountry = null, string QuantityPlaces = null)
        {
            int mapsPerPage = 10;

            if (!int.TryParse(Page, out int pagesToSkip))
                throw new InvalidOperationException("Nieprawidłowy Page");

            var query = _mapRepository.GetAllMapsWithCountries();

            // Filtrowanie tylko opublikowanych map
            query = query.Where(m => m.MapUploaded == true);

            // Filtrowanie na podstawie SeatchFilters
            if (SearchFilters != null && SearchFilters.Any())
            {
                foreach (var filter in SearchFilters)
                {
                    query = query.Where(m => m.MapName.Contains(filter) || m.MapTags.Any(tag => tag.TagName.Contains(filter)));
                }
            }

            // Filtrowanie na podstawie wybranego kraju
            if (!string.IsNullOrEmpty(ChosenCountry))
            {
                query = query.Where(m => m.MapCountries.Any(mc => mc.Country.CountryName == ChosenCountry));
            }

            // Filtrowanie po liczbie miejsc na mapie
            if (!string.IsNullOrEmpty(QuantityPlaces))
            {
                switch (QuantityPlaces)
                {
                    case "1": // Mniej niż 5 miejsc
                        query = query.Where(m => m.MapPlacesQuantity < 5);
                        break;
                    case "2": // 5-10 miejsc
                        query = query.Where(m => m.MapPlacesQuantity >= 5 && m.MapPlacesQuantity <= 10);
                        break;
                    case "3": // 10-15 miejsc
                        query = query.Where(m => m.MapPlacesQuantity > 10 && m.MapPlacesQuantity <= 15);
                        break;
                    case "4": // 15-20 miejsc
                        query = query.Where(m => m.MapPlacesQuantity > 15 && m.MapPlacesQuantity <= 20);
                        break;
                    case "5": // Więcej niż 20 miejsc
                        query = query.Where(m => m.MapPlacesQuantity > 20);
                        break;
                }
            }

            query = query.Skip((pagesToSkip - 1) * mapsPerPage).Take(mapsPerPage);

            var filteredMaps = await query.Select(m => new MapDto
            {
                MapId = m.MapId,
                MapName = m.MapName,
                MapDescription = m.MapDescription,
                MapLikes = m.MapLikes,
                MapCreatorName = m.MapCreator.UserName,
                MapCreatorId = m.MapCreatorId,
                MapCreatorPicture = m.MapCreator.UserProfilePicture,
                MapThumbnail = m.MapThumbnail

            }).ToListAsync();

            foreach (var map in filteredMaps)
            {
                if(map.MapCreatorPicture != null)
                    map.MapCreatorPicture = await _firebaseStorageService.GetSignedUrlAsync(map.MapCreatorPicture);

                if(map.MapThumbnail != null)
                    map.MapThumbnail = await _firebaseStorageService.GetSignedUrlAsync(map.MapThumbnail);
            }

            return filteredMaps;
        }

        /// <summary>
        /// Pobiera wyznaczone opublikowane mapy użytkownika
        /// </summary>
        /// <param name="id">Id użytkownika</param>
        /// <param name="page">numer strony do pobrania</param>
        /// <returns>Listę map</returns>
        public async Task<List<MapDto>> GetPublicatedMapsByUserIdAsyncAndPageNumber(string id, string page)
        {
            if (!int.TryParse(id, out int userId))
                throw new InvalidOperationException("Nieprawidłowe ID użytkownika.");
            if (!int.TryParse(page, out int pageNumber))
                throw new InvalidOperationException("Nieoczekiwany błąd");

            if (pageNumber == 0)
                pageNumber = 1;

            int pageSize = 20;

            List<Map> maps = await _mapRepository.GetMapsByUserWithPaginationAsync(userId, pageNumber, pageSize, true);

            List<MapDto> mapsDto = new List<MapDto>();
            foreach (var map in maps)
            {
                MapDto mapDto = new MapDto()
                {
                    MapId = map.MapId,
                    MapName = map.MapName,
                    MapDescription = map.MapDescription,
                    MapThumbnail = map.MapThumbnail,
                    MapLikes = map.MapLikes,
                    MapPlacesQuantity = map.MapPlacesQuantity,
                };

                mapsDto.Add(mapDto);
            }

            return mapsDto;
        }

        /// <summary>
        /// Pobiera wyznaczone nieopublikowane mapy użytkownika
        /// </summary>
        /// <param name="id">Id użytkownika</param>
        /// <param name="page">numer strony do pobrania</param>
        /// <returns>Listę map</returns>
        public async Task<List<MapDto>> GetPrivateMapsByUserIdAsyncAndPageNumber(string id, string page)
        {
            if (!int.TryParse(id, out int userId))
                throw new InvalidOperationException("Nieprawidłowe ID użytkownika.");
            if (!int.TryParse(page, out int pageNumber))
                throw new InvalidOperationException("Nieoczekiwany błąd");

            if (pageNumber == 0)
                pageNumber = 1;

            int pageSize = 20;

            List<Map> maps = await _mapRepository.GetMapsByUserWithPaginationAsync(userId, pageNumber, pageSize, false);

            List<MapDto> mapsDto = new List<MapDto>();
            foreach (var map in maps)
            {
                MapDto mapDto = new MapDto()
                {
                    MapId = map.MapId,
                    MapName = map.MapName,
                    MapDescription = map.MapDescription,
                    MapThumbnail = map.MapThumbnail,
                    MapLikes = map.MapLikes,
                    MapPlacesQuantity = map.MapPlacesQuantity,
                };

                mapsDto.Add(mapDto);
            }

            return mapsDto;
        }

        /// <summary>
        /// Metoda aktualizująca mapę w bazie
        /// </summary>
        /// <param name="map">Mapa</param>
        public async Task UpdateMapAsync(Map map)
        {
            await _mapRepository.UpdateAsync(map);
        }

        /// <summary>
        /// Pobiera nieopublikowane mapy użytkownika
        /// </summary>
        /// <param name="id">id użytkownika</param>
        /// <returns>Lista nieopublikowanych map dto</returns>
        public async Task<List<MapDto>> GetPrivateMapsByUserIdAsync(string id)
        {
            if (id.IsNullOrEmpty())
                throw new ArgumentNullException("Błędne id");

            if (!int.TryParse(id, out var userId))
                throw new ArgumentNullException("Nieprawidłowe ID mapy.");

            List<Map>? privateMaps = await _mapRepository.GetAllUserPrivateMaps(userId);
            List<MapDto>? privateUserMaps = new List<MapDto>();
            if (privateMaps.Any())
            {
                foreach (var map in privateMaps)
                {
                    MapDto mapDto = new MapDto()
                    {
                        MapId = map.MapId,
                        MapName = map.MapName,
                        MapDescription = map.MapDescription,
                        MapThumbnail = map.MapThumbnail,
                        MapLikes = map.MapLikes,
                        MapPlacesQuantity = map.MapPlacesQuantity,
                    };
                    privateUserMaps.Add(mapDto);
                }
            }
            return privateUserMaps;
        }

        /// <summary>
        /// Usuwa mapę oraz wszystkie powiązane dane na podstawie jej ID
        /// </summary>
        /// <param name="mapId">ID mapy do usunięcia</param>
        /// <returns></returns>
        public async Task DeleteMapByIdAsync(int mapId)
        {
            if (mapId <= 0)
                throw new InvalidDataException("Niepoprawny identyfikator mapy.");

            var map = await _mapRepository.GetByIdWithDetailsAsync(mapId);

            if (map == null)
            {
                throw new InvalidOperationException("Mapa o podanym ID nie istnieje.");
            }

            if (map.MapPlaces != null)
            {
                await _placeRepository.RemoveRangeAsync(map.MapPlaces);
            }

            if (map.MapTags != null)
            {
                foreach (var tag in map.MapTags)
                {
                    tag.MapsWithThisTag.Remove(map);
                }
            }

            if (map.MapCountries != null)
            {
                await _mapCountryRepository.RemoveRangeAsync(map.MapCountries);
            }

            if (map.UserLikes != null)
            {
                await _userMapLikeRepository.RemoveRangeAsync(map.UserLikes);
            }

            var creator = await _userRepository.GetByIdWithMapsAsync(map.MapCreatorId);

            if (creator != null && creator.UserMaps != null)
            {
                creator.UserMaps.Remove(map);
            }

            if (map.MapThumbnail != null)
                await _firebaseStorageService.DeleteFileAsync(map.MapThumbnail);

            await _mapRepository.DeleteAsync(map);
        }

        /// <summary>
        /// Publikuje mapę o podanym id
        /// </summary>
        /// <param name="mapId">id mapy</param>
        public async Task PublishMapAsync(int mapId)
        {
            var map = await _mapRepository.GetByIdAsync(mapId);
            if (map == null)
                throw new InvalidOperationException("Nie znaleziono mapy o podanym id");

            map.MapUploaded = true;

            await _mapRepository.UpdateAsync(map);
        }

        /// <summary>
        /// Przenosi mapę do wersji roboczych
        /// </summary>
        /// <param name="mapId"></param>
        public async Task MoveMapToDraftAsync(int mapId)
        {
            var map = await _mapRepository.GetByIdAsync(mapId);
            if (map == null)
                throw new InvalidOperationException("Nie znaleziono mapy o podanym id");

            map.MapUploaded = false;

            await _mapRepository.UpdateAsync(map);
        }

        /// <summary>
        /// Zwiększa liczbę polubień mapy
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task IncreaseLikesAsync(int mapId)
        {
            try
            {
                var map = await _mapRepository.GetByIdAsync(mapId);
                if (map == null)
                    throw new InvalidOperationException("Mapa nie została znaleziona.");

                map.MapLikes++;
                await _mapRepository.UpdateAsync(map);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd w MapService.IncreaseLikesAsync");
                throw;
            }
        }

        /// <summary>
        /// Zmniejsza liczbę polubień mapy
        /// </summary>
        /// <param name="mapId">ID mapy.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DecreaseLikesAsync(int mapId)
        {
            if (mapId <= 0)
                throw new ArgumentNullException("Błędne ID mapy.");

            try
            {
                var map = await _mapRepository.GetByIdAsync(mapId);
                if (map == null)
                    throw new InvalidOperationException("Mapa nie została znaleziona.");

                if (map.MapLikes > 0)
                {
                    map.MapLikes--;
                    await _mapRepository.UpdateAsync(map);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd w MapService.DecreaseLikesAsync");
                throw;
            }
        }

    }
}
