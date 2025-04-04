using JurMaps.Model;

namespace JurMaps.Repository.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Pobiera rolę po ID.
        /// </summary>
        /// <param name="roleId">Identyfikator roli</param>
        /// <returns>Znaleziona rola lub null</returns>
        Task<Role?> GetRoleByRoleIdAsync(int roleId);
    }
}
