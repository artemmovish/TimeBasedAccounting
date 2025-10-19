using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Interfaces
{
    /// <summary>
    /// Сервис для работы с пользователями
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        Task<User?> AuthenticateAsync(string login, string password);

        /// <summary>
        /// Получить пользователя по идентификатору
        /// </summary>
        Task<User> GetUserByIdAsync(int userId);

        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}