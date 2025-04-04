using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Repository.Interfaces;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static NetTopologySuite.Geometries.Utilities.GeometryMapper;

namespace JurMaps.Services
{
    public class UserMapLikesService : IUserMapLikesService
    {
        private readonly IUserMapLikeRepository _userMapLikeRepository;
        private readonly IUserService _userService;
        private readonly IMapService _mapService;
        public UserMapLikesService(IUserMapLikeRepository userMapLikeRepository, IUserService userService, IMapService mapService)
        {
            _userMapLikeRepository = userMapLikeRepository;
            _userService = userService;
            _mapService = mapService;
        }

        /// <summary>
        /// Metoda wyszukujący czy użytkownik polubił już mapę
        /// </summary>
        /// <param name="mapId">Id mapy</param>
        /// <param name="userId">Id użytkownika</param>
        /// <returns>Znalezione połączenie</returns>
        public async Task<UserMapLike?> WasMapLikedByUserAsync(int mapId, int userId)
        {

            if (mapId <= 0 || userId <= 0)
                throw new Exception("Nieprawidłowe ID użytkownika lub mapy.");
            /*            if (userId <= 0)
                            throw new ArgumentException("Nie można zidentyfikować użytkownika.");

                        var user = await _userService.GetUserFromIdAsync(userId);
                        if (user == null)
                            throw new ArgumentNullException("Nie odnaleziono użytkownika.");

                        var map = await _mapService.GetMapByIdAsync(mapId);
                        if (map == null)
                            throw new ArgumentNullException("Nie odnaleziono mapy.");*/

            return await _userMapLikeRepository.GetUserMapLikeByUserIdAndMapId(userId, mapId);
        }

        /// <summary>
        /// Dodaje nowy obiekt userMapLike do bazy
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="userId">ID użytkownika</param>
        /// <returns></returns>
        public async Task AddLikeAsync(int mapId, int userId)
        {
            var existingLike = await WasMapLikedByUserAsync(mapId, userId);
            if (existingLike != null)
                throw new InvalidOperationException("Użytkownik już polubił tę mapę.");

            try
            {
                await _userMapLikeRepository.AddAsync(new UserMapLike
                {
                    UserId = userId,
                    MapId = mapId
                });
                await _mapService.IncreaseLikesAsync(mapId);
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Błąd podczas dodawania polubienia: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Wystąpił nieoczekiwany błąd: " + ex.Message);
            }
        }

        /// <summary>
        /// Usuwa obiekt userMapLike z bazy, usuwając polubienie mapy przez użytkownika
        /// </summary>
        /// <param name="mapId">ID mapy</param>
        /// <param name="userId">ID użytkownika</param>
        /// <returns></returns>
        public async Task RemoveLikeAsync(int mapId, int userId)
        {
            try
            {
                UserMapLike? userMapLike = await WasMapLikedByUserAsync(mapId, userId);
                if (userMapLike == null)
                    throw new InvalidDataException("Użytkownik nie polubił tej mapy.");

                await _userMapLikeRepository.RemoveAsync(userMapLike);

                await _mapService.DecreaseLikesAsync(mapId);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("Polubienie nie istnieje.");
            }
            catch (Exception ex)
            {
                throw new Exception("Wystąpił nieoczekiwany błąd: " + ex.Message);
            }
        }


        /// <summary>
        /// Pobiera wyznaczone polubione mapy użytkownika
        /// </summary>
        /// <param name="id">id użytkownika</param>
        /// <param name="page">numer strony do pobrania</param>
        /// <returns>lista map</returns>
        public async Task<List<MapDto>> GetLikedMapsByUserIdAndPageNumberAsync(string id, string page)
        {
            if (!int.TryParse(id, out int userId))
                throw new ArgumentException("Nieprawidłowy format ID");

            if (!int.TryParse(page, out int pageNumber))
                throw new InvalidOperationException("Nieoczekiwany błąd");

            if (pageNumber == 0)
                pageNumber = 1;

            int pageSize = 20;

            var userMapLikes = await _userMapLikeRepository.GetLikedMapsByUserIdAndPageNumberAsync(userId, pageNumber, pageSize);

            List<MapDto> likedMaps = new List<MapDto>();
            foreach (var userMapLike in userMapLikes)
            {
                var map = userMapLike.Map;
                if (map != null)
                {
                    var mapDto = new MapDto
                    {
                        MapId = map.MapId,
                        MapName = map.MapName,
                        MapDescription = map.MapDescription,
                        MapThumbnail = map.MapThumbnail,
                        MapLikes = map.MapLikes,
                    };
                    likedMaps.Add(mapDto);
                }
            }

            return likedMaps;
        }

    }
}
