using JurMaps.Model;
using JurMaps.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JurMaps.Repository
{
    /// <summary>
    /// Repozytorium odpowiedzialne za operacje na obiekcie Role w bazie danych.
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly MyDbContext _context;

        public RoleRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Pobiera rolę po ID.
        /// </summary>
        /// <param name="roleId">Identyfikator roli</param>
        /// <returns>Znaleziona rola lub null</returns>
        public async Task<Role?> GetRoleByRoleIdAsync(int roleId)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
        }
    }
}
