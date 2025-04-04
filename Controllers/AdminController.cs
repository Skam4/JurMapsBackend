using JurMaps.Model;
using JurMaps.Model.DTO;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace JurMaps.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : BaseController<AdminController>
    {
        public AdminController(
            IUserService userService,
            IMapService mapService,
            ILogger<AdminController> logger)
            : base(userService, logger, mapService) { }

        /// <summary>
        /// Pobiera wszystkich użytkowników
        /// </summary>
        /// <returns>Zwraca listę UserDto</returns>
        [HttpGet("getallusers")]
        public async Task<ControllerResult<List<UserDto>>> GetAllUsers()
        {
            try
            {
                List<UserDto> users = await _userService.GetAllUsersAsync();
                return ControllerResult<List<UserDto>>.Ok(users, "Lista użytkowników pobrana pomyślnie.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Błąd w AdminController.GetAllUsers");
                return ControllerResult<List<UserDto>>.BadRequest($"Błąd podczas pobierania użytkowników: {ex.Message}");
            }
        }

        /// <summary>
        /// Pobiera użytkownika na podstawie identyfikatora.
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <returns>True, jeżeli udało się pobrać użytkownika</returns>
        [HttpGet("{userId}")] //Dlaczego ten route nie ma nazwy?
        public async Task<ControllerResult<User>> GetUser(int userId)
        {
            try
            {
                User? user = await _userService.GetUserFromIdAsync(userId);
                return user == null
                    ? ControllerResult<User>.NotFound("Nie znaleziono użytkownika o podanym ID.")
                    : ControllerResult<User>.Ok(user, "Dane użytkownika pobrane pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AdminController.GetUser");
                return ControllerResult<User>.BadRequest($"Błąd podczas pobierania użytkownika: {ex.Message}");
            }
        }

        /// <summary>
        /// Tworzy nowego użytkownika
        /// </summary>
        /// <param name="newUser">Obiekt nowego użytkownika</param>
        /// <returns>True, jeżeli udało się stworzyć użytkownika</returns>
        [HttpPost("createuser")]
        public async Task<ControllerResult<bool>> CreateUser([FromBody] RegisterDto newUser)
        {
            try
            {
                await _userService.RegisterUserAsync(newUser);
                return ControllerResult<bool>.Ok(true, "Użytkownik stworzony pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AdminController.CreateUser");
                return ControllerResult<bool>.ServerError($"Wystąpił błąd podczas tworzenia użytkownika: {ex.Message}");
            }
        }

        /// <summary>
        /// Aktualizuje dane użytkownika.
        /// </summary>
        /// <param name="updateUserDto">Zaktualizowane dane użytkownika</param>
        /// <returns>True, jeżeli udało się użytkownika zaktualizować</returns>
        [HttpPut("updateuser")]
        public async Task<ControllerResult<bool>> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                await _userService.UpdateUserAsync(updateUserDto);
                return ControllerResult<bool>.Ok(true, "Pomyślnie zaktualizowano użytkownika.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AdminController.UpdateUser");
                return ControllerResult<bool>.ServerError($"Wystąpił błąd podczas aktualizowania użytkownika: {ex.Message}");
            }
        }

        /// <summary>
        /// Usuwa użytkownika na podstawie identyfikatora.
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <returns>True, jeżeli udało się użytkownika usunąć</returns>
        [HttpDelete("deleteuser/{userId}")]
        public async Task<ControllerResult<bool>> DeleteUserAsync(int userId)
        {
            try
            {
                await _userService.DeleteUserAsync(userId);
                return ControllerResult<bool>.Ok(true, "Użytkownik usunięty pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AdminController.DeleteUserAsync");
                return ControllerResult<bool>.ServerError($"Wystąpił błąd podczas usuwania użytkownika: {ex.Message}");
            }
        }

        /// <summary>
        /// Usuwa mapę na podstawie identyfikatora.
        /// </summary>
        /// <param name="mapId">ID mapy.</param>
        /// <returns>True, jeżeli mapę udało się usunąć</returns>
        [HttpDelete("deletemap/{mapId}")]
        public async Task<ControllerResult<bool>> DeleteMapByIdAsync(int mapId)
        {
            try
            {
                await _mapService.DeleteMapByIdAsync(mapId);
                return ControllerResult<bool>.Ok(true, "Mapa usunięta pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AdminController.DeleteMapByIdAsync");
                return ControllerResult<bool>.ServerError($"Wystąpił błąd podczas usuwania mapy: {ex.Message}");
            }
        }

        /// <summary>
        /// Pobiera wszystkie mapy.
        /// </summary>
        /// <returns>Lista map</returns>
        [HttpGet("getmaps")]
        public async Task<ControllerResult<List<MapDto>>> GetMaps()
        {
            try
            {
                List<MapDto> mapList = await _mapService.GetMapsAsync();
                return mapList == null || !mapList.Any()
                    ? ControllerResult<List<MapDto>>.NotFound("Nie znaleziono żadnych map.")
                    : ControllerResult<List<MapDto>>.Ok(mapList, "Lista map pobrana pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w AdminController.GetMaps");
                return ControllerResult<List<MapDto>>.ServerError($"Wystąpił błąd podczas pobierania map: {ex.Message}");
            }
        }
    }


}
