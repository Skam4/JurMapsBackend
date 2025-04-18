using JurMaps.Services.Interfaces;
using JurMaps.Model;
using NetTopologySuite.Geometries;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using JurMaps.Repository.Interfaces;
using JurMaps.Model.DTO;
using NetTopologySuite.GeometriesGraph;
using Microsoft.AspNetCore.Mvc;
using JurMaps.Shared;

namespace JurMaps.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly IPlaceRepository _placeRepository;
        private readonly IMapRepository _mapRepository;
        private readonly IMapCountryRepository _mapCountryRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly Helpers _helpers;

        public PlaceService(
            IFirebaseStorageService firebaseStorageService,
            IPlaceRepository placeRepository,
            IMapRepository mapRepository,
            IMapCountryRepository mapCountryRepository,
            ICountryRepository countryRepository,
            Helpers helpers)
        {
            _firebaseStorageService = firebaseStorageService;
            _placeRepository = placeRepository;
            _mapRepository = mapRepository;
            _mapCountryRepository = mapCountryRepository;
            _countryRepository = countryRepository;
            _helpers = helpers;
        }

        /// <summary>
        /// Zapisywanie markera do bazy
        /// </summary>
        /// <param name="marker">Obiekt markera</param>
        /// <param name="mapId">id mapy</param>
        /// <returns>Identifikator markera</returns>
        public async Task<int> AddMarkerAsync(MarkerDto marker, int mapId)
        {
            if (marker?.Position == null)
                throw new InvalidDataException("Nieprawidłowe dane.");

            Map? map = await _mapRepository.GetByIdWithPlacesAsync(mapId);
            if (map == null)
                throw new InvalidOperationException("Podana mapa nie istnieje.");

            if (!string.IsNullOrEmpty(marker.PlaceCountry))
            {
                var mapCountry = map.MapCountries.FirstOrDefault(mc =>
                    mc.Country.CountryName.Equals(marker.PlaceCountry));

                if (mapCountry != null)
                {
                    // Jeśli kraj już istnieje, zwiększamy licznik połączeń
                    mapCountry.ConnectionCount++;
                    await _mapCountryRepository.UpdateAsync(mapCountry);
                }
                else
                {
                    // Jeśli kraj nie istnieje, tworzymy nowy obiekt Country i nowy MapCountry
                    var country = await _countryRepository.GetByNameAsync(marker.PlaceCountry) ?? new Country(marker.PlaceCountry);

                    if (country.CountryId == 0)
                        await _countryRepository.AddAsync(country);

                    var newMapCountry = new MapCountry
                    {
                        Map = map,
                        CountryId = country.CountryId,
                        Country = country,
                        MapId = map.MapId,
                        ConnectionCount = 1
                    };
                    await _mapCountryRepository.AddAsync(newMapCountry);
                    map.MapCountries.Add(newMapCountry);
                    await _countryRepository.UpdateAsync(country);
                }
            }

            Place place = new Place
            {
                PlacePosition = new Point(marker.Position.X, marker.Position.Y),
                PlaceType = "Marker",
                PlaceMap = map,
                PlaceColor = marker.PlaceColor,
            };
            if (marker.PlaceCountry != null)
            {
                place.PlaceCountry = marker.PlaceCountry;
            }

            await _placeRepository.AddAsync(place);

            map.MapPlacesQuantity++;
            map.MapPlaces.Add(place);

            await _mapRepository.UpdateAsync(map);

            return place.PlaceId;
        }

        public async Task<int> AddCircleAsync(string name, string description, Point circle, double radius, int mapId)
        {

            var map = await _mapRepository.GetByIdAsync(mapId);
            if (map == null)
            {
                throw new ArgumentException("Map id is wrong");
            } 
            else
            {
                Place place = new Place();
                place.PlacePosition = circle;
                //place.PlaceRadius = radius;
                place.PlaceType = "Circle";
                place.PlaceMap = map;
                place.PlaceName = name;
                place.PlaceDescription = description;

                await _placeRepository.AddAsync(place);
                map.MapPlacesQuantity++;
                await _mapRepository.UpdateAsync(map);

                return place.PlaceId;
            }
        }

        public async Task<int> AddRectangleAsync(string name, string description, List<Point> points, int mapId)
        {
            if (points.Count != 2)
            {
                throw new ArgumentException("Rectangle must have exactly 2 points.");
            }

            var map = await _mapRepository.GetByIdAsync(mapId);
            if (map == null)
            {
                throw new ArgumentException("Map id is wrong");
            }

            Place place = new Place();
            place.PlaceType = "Rectangle";

            var lowerLeft = points[0];
            var upperRight = points[1];

            var lowerRight = new Point(upperRight.X, lowerLeft.Y);
            var upperLeft = new Point(lowerLeft.X, upperRight.Y);

            var rectangleCoordinates = new Coordinate[]
            {
                new Coordinate(lowerLeft.X, lowerLeft.Y),
                new Coordinate(lowerRight.X, lowerRight.Y),
                new Coordinate(upperRight.X, upperRight.Y),
                new Coordinate(upperLeft.X, upperLeft.Y),
                new Coordinate(lowerLeft.X, lowerLeft.Y)
            };

            var rectangleRing = new LinearRing(rectangleCoordinates);
            var rectanglePolygon = new Polygon(rectangleRing);

            //place.Bounds = rectanglePolygon;
            place.PlaceMap = map;
            place.PlaceName = name;
            place.PlaceDescription = description;

            await _placeRepository.AddAsync(place);
            map.MapPlacesQuantity++;
            await _mapRepository.UpdateAsync(map);

            return place.PlaceId;
        }

        /// <summary>
        /// Zapisywanie informacji o miejscu do bazy
        /// </summary>
        /// <param name="placeId">id miejsca</param>
        /// <param name="placeName">nazwa miejsca</param>
        /// <param name="placeDescription">opis miejsca</param>
        /// <param name="placePhoto1">zdjęcie pierwsze</param>
        /// <param name="placePhoto2">zdjęcie drugie</param>
        /// <param name="placePhoto3">zdjęcie trzecie</param>
        /// <param name="placeColor">kolor oznaczenia miejsca</param>
        /// /// <param name="changePlacePhoto1">Oznaczenie czy zdjęcie zostało zmienione</param>
        /// /// <param name="changePlacePhoto2">Oznaczenie czy zdjęcie zostało zmienione</param>
        /// /// <param name="changePlacePhoto3">Oznaczenie czy zdjęcie zostało zmienione</param>
        public async Task SavePlaceInfoAsync(
            int placeId,
            string placeName,
            string placeDescription,
            IFormFile? placePhoto1,
            IFormFile? placePhoto2,
            IFormFile? placePhoto3,
            string placeColor,
            bool changePlacePhoto1 = false,
            bool changePlacePhoto2 = false,
            bool changePlacePhoto3 = false)
        {
            if (string.IsNullOrEmpty(placeName) || string.IsNullOrEmpty(placeDescription))
                throw new ArgumentNullException("Nazwa i opis miejsca są wymagane.");

            float toxicityScore = await _helpers.AnalyzeText(placeName);

            if (toxicityScore >= 0.10)
            {
                throw new InvalidOperationException("Nazwa miejsca jest zbyt toksyczna i nie może zostać zapisana.");
            }

            toxicityScore = await _helpers.AnalyzeText(placeDescription);

            if (toxicityScore >= 0.10)
            {
                throw new InvalidOperationException("Opis miejsca jest zbyt toksyczny i nie może zostać zapisany.");
            }

            var place = await _placeRepository.GetPlaceByIdAsync(placeId);
            if (string.IsNullOrEmpty(placeName))
                throw new InvalidOperationException("Nazwa miejsca jest wymagana");

            place.PlaceName = placeName;
            place.PlaceDescription = placeDescription;
            place.PlaceColor = placeColor;

            // Zdjęcie 1
            if (changePlacePhoto1 && placePhoto1.Length > 0)
            {
                if (!string.IsNullOrEmpty(place.PlacePhoto1))
                {
                    await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto1);
                }
                var url1 = await _firebaseStorageService.UploadFileAsync(placePhoto1);
                place.PlacePhoto1 = url1;
            }
            else if (changePlacePhoto1 && (placePhoto1 == null || placePhoto1.Length == 0))
            {
                if (!string.IsNullOrEmpty(place.PlacePhoto1))
                {
                    await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto1);
                    place.PlacePhoto1 = null;
                }
            }

            // Zdjęcie 2
            if (changePlacePhoto2 && placePhoto2.Length > 0)
            {
                if (!string.IsNullOrEmpty(place.PlacePhoto2))
                {
                    await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto2);
                }
                var url2 = await _firebaseStorageService.UploadFileAsync(placePhoto2);
                place.PlacePhoto2 = url2;
            }
            else if (changePlacePhoto2 && (placePhoto2 == null || placePhoto2.Length == 0))
            {
                if (!string.IsNullOrEmpty(place.PlacePhoto2))
                {
                    await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto2);
                    place.PlacePhoto2 = null;
                }
            }

            // Zdjęcie 3
            if (changePlacePhoto3 && placePhoto3.Length > 0)
            {
                if (!string.IsNullOrEmpty(place.PlacePhoto3))
                {
                    await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto3);
                }
                var url3 = await _firebaseStorageService.UploadFileAsync(placePhoto3);
                place.PlacePhoto3 = url3;
            }
            else if (changePlacePhoto3 && (placePhoto3 == null || placePhoto3.Length == 0))
            {
                if (!string.IsNullOrEmpty(place.PlacePhoto3))
                {
                    await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto3);
                    place.PlacePhoto3 = null;
                }
            }
            if (placePhoto1?.Length != 0)
                await _helpers.CheckImageAsync(placePhoto1);
            if (placePhoto2?.Length != 0)
                await _helpers.CheckImageAsync(placePhoto2);
            if (placePhoto3?.Length != 0)
                await _helpers.CheckImageAsync(placePhoto3);

            await _placeRepository.UpdateAsync(place);
        }



        /// <summary>
        /// Pobiera obiekt miejsca po identyfikatorze
        /// </summary>
        /// <param name="id">ID miejsca</param>
        /// <returns>Znalezione miejsce</returns>
        public async Task<Place?> GetPlaceByIdAsync(int id)
        {
            return await _placeRepository.GetPlaceByIdAsync(id);
        }

        /// <summary>
        /// Aktualizuje położenie markera
        /// </summary>
        /// <param name="markerId">ID markera</param>
        /// <param name="position">Nowe położenie markera</param>
        /// <returns></returns>
        public async Task UpdateMarkerPositionAsync(int markerId, PositionDto position)
        {
            if (position == null || position.X == 0 || position.Y == 0)
                throw new ArgumentNullException("Nieprawidłowa pozycja markera.");

            Place? place = await _placeRepository.GetPlaceByIdAsync(markerId);
            if (place == null)
                throw new ArgumentNullException("Nie znaleziono markera o podanym ID.");

            place.PlacePosition = new Point(position.X, position.Y);

            await _placeRepository.UpdateAsync(place);
        }

        /// <summary>
        /// Usuwa miejsce po id
        /// </summary>
        /// <param name="placeId">id miejsca</param>
        public async Task DeleteAsync(int placeId)
        {
            Place? place = await _placeRepository.GetPlaceByIdAsync(placeId);
            if (place == null)
                throw new ArgumentException("Nie znaleziono miejsca w bazie");

            Map? placeMap = await _mapRepository.GetByIdAsync(place.MapId);
            if(placeMap == null)
                throw new ArgumentException("Nie znaleziono przypisanej mapy miejsca");

            placeMap.MapPlacesQuantity--;

            if(place.PlacePhoto1 != null)
                await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto1);

            if (place.PlacePhoto2 != null)
                await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto2);

            if (place.PlacePhoto3 != null)
                await _firebaseStorageService.DeleteFileAsync(place.PlacePhoto3);

            string? placeCountry = place.PlaceCountry;

            if (!string.IsNullOrEmpty(placeCountry))
            {
                var mapCountry = placeMap.MapCountries?
                    .FirstOrDefault(mc => mc.Country.CountryName == placeCountry);

                if (mapCountry != null)
                {
                    mapCountry.ConnectionCount--;

                    if (mapCountry.ConnectionCount <= 0)
                    {
                        await _mapCountryRepository.RemoveAsync(mapCountry);
                    }
                }
            }

            await _placeRepository.RemoveAsync(place);
        }


    }
}
