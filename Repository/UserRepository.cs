using JurMaps.Model;
using JurMaps.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using RTools_NTS.Util;

namespace JurMaps.Repository
{
    /// <summary>
    /// Repozytorium odpowiedzialne za operacje na obiekcie User w bazie danych.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _context;

        /// <summary>
        /// Inicjalizuje nowe repozytorium użytkowników z kontekstem bazy danych.
        /// </summary>
        /// <param name="context">Kontekst bazy danych.</param>
        public UserRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Dołącza obiekt użytkownika do kontekstu, jeśli nie jest już lokalnie załadowany.
        /// </summary>
        /// <param name="user">Obiekt użytkownika do dołączenia.</param>
        public void Attach(User user)
        {
            if (_context.Users.Local.All(u => u.UserId != user.UserId))
            {
                _context.Users.Attach(user);
            }
        }

        /// <summary>
        /// Aktualizuje istniejący obiekt użytkownika w bazie danych.
        /// </summary>
        /// <param name="user">Obiekt użytkownika do zaktualizowania.</param>
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera użytkownika wraz z przypisanymi mapami na podstawie identyfikatora użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        public async Task<User?> GetByIdWithMapsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserMaps)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        /// <summary>
        /// Pobiera użytkownika na podstawie adresu e-mail.
        /// </summary>
        /// <param name="email">Adres e-mail użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(v => v.UserEmail == email);
        }

        /// <summary>
        /// Pobiera użytkownika wraz z jego rolą na podstawie adresu e-mail.
        /// </summary>
        /// <param name="email">Adres e-mail użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        public async Task<User?> GetUserByEmailWithRoleAsync(string email)
        {
            return await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(v => v.UserEmail == email);
        }

        /// <summary>
        /// Pobiera użytkownika na podstawie nazwy użytkownika.
        /// </summary>
        /// <param name="userName">Nazwa użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        /// <summary>
        /// Dodaje nowego użytkownika do bazy danych.
        /// </summary>
        /// <param name="user">Obiekt użytkownika do dodania.</param>
        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera użytkownika na podstawie tokenu resetowania hasła.
        /// </summary>
        /// <param name="token">Token resetowania hasła.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        public async Task<User?> GetUserByTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(v => v.ResetPasswordToken == token);
        }

        /// <summary>
        /// Pobiera użytkownika wraz z jego polubionymi mapami na podstawie identyfikatora użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        public async Task<User?> GetUserByIdWithUserLikedMapsAsync(int userId)
        {
            return await _context.Users.Include(v => v.UserLikedMaps).FirstOrDefaultAsync(v => v.UserId == userId);
        }

        /// <summary>
        /// Pobiera użytkownika wraz z jego mapami na podstawie identyfikatora użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        public async Task<User?> GetUserByIdWithUserMapsAsync(int userId)
        {
            return await _context.Users.Include(v => v.UserMaps).FirstOrDefaultAsync(u => u.UserId == userId);
        }

        /// <summary>
        /// Pobiera listę wszystkich użytkowników.
        /// </summary>
        /// <returns>Lista użytkowników.</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Usuwa użytkownika z bazy danych.
        /// </summary>
        /// <param name="user">Obiekt użytkownika do usunięcia.</param>
        public async Task RemoveAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera użytkownika na podstawie identyfikatora użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.Include(m => m.UserMaps).FirstOrDefaultAsync(u => u.UserId == userId);
        }

        /// <summary>
        /// Pobiera token służący do weryfikacji konta użytkownika
        /// </summary>
        /// <param name="token">Token weryfikujący konto.</param>
        /// <returns>Użytkownik posiadający podany token.</returns>
        public async Task<User?> GetUserByVerificationTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
        }
    }


}
