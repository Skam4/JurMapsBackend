using JurMaps.Model;

namespace JurMaps.Repository.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Dołącza obiekt użytkownika do kontekstu, jeśli nie jest już lokalnie załadowany.
        /// </summary>
        /// <param name="user">Obiekt użytkownika do dołączenia.</param>
        void Attach(User user);

        /// <summary>
        /// Aktualizuje istniejący obiekt użytkownika w bazie danych.
        /// </summary>
        /// <param name="user">Obiekt użytkownika do zaktualizowania.</param>
        Task UpdateAsync(User user);

        /// <summary>
        /// Pobiera użytkownika wraz z przypisanymi mapami na podstawie identyfikatora użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        Task<User?> GetByIdWithMapsAsync(int userId);

        /// <summary>
        /// Pobiera użytkownika na podstawie adresu e-mail.
        /// </summary>
        /// <param name="email">Adres e-mail użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Pobiera użytkownika wraz z jego rolą na podstawie adresu e-mail.
        /// </summary>
        /// <param name="email">Adres e-mail użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        Task<User?> GetUserByEmailWithRoleAsync(string email);

        /// <summary>
        /// Pobiera użytkownika na podstawie nazwy użytkownika.
        /// </summary>
        /// <param name="userName">Nazwa użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        Task<User?> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// Dodaje nowego użytkownika do bazy danych.
        /// </summary>
        /// <param name="user">Obiekt użytkownika do dodania.</param>
        Task AddAsync(User user);

        /// <summary>
        /// Pobiera użytkownika na podstawie tokenu resetowania hasła.
        /// </summary>
        /// <param name="token">Token resetowania hasła.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        Task<User?> GetUserByTokenAsync(string token);

        /// <summary>
        /// Pobiera użytkownika wraz z jego polubionymi mapami na podstawie identyfikatora użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        Task<User?> GetUserByIdWithUserLikedMapsAsync(int userId);

        /// <summary>
        /// Pobiera użytkownika wraz z jego mapami na podstawie identyfikatora użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        Task<User?> GetUserByIdWithUserMapsAsync(int userId);

        /// <summary>
        /// Pobiera listę wszystkich użytkowników.
        /// </summary>
        /// <returns>Lista użytkowników.</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Usuwa użytkownika z bazy danych.
        /// </summary>
        /// <param name="user">Obiekt użytkownika do usunięcia.</param>
        Task RemoveAsync(User user);

        /// <summary>
        /// Pobiera użytkownika na podstawie identyfikatora użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika.</param>
        /// <returns>Obiekt użytkownika lub null, jeśli nie znaleziono.</returns>
        Task<User?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Pobiera token służący do weryfikacji konta użytkownika
        /// </summary>
        /// <param name="token">Token weryfikujący konto.</param>
        /// <returns>Użytkownik posiadający podany token.</returns>
        Task<User?> GetUserByVerificationTokenAsync(string token);
    }
}
