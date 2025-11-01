using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Models;
using TimeBasedAccounting.Core.Services;
using static TimeBasedAccounting.Core.Services.UserService;

namespace TimeBasedAccounting.Core.Interfaces
{
    /// <summary>
    /// Сервис для работы с пользователями
    /// </summary>
    public interface IUserService
    {
        Task<LoginResult> UserLoginAsync(string login, string password);
        Task<AddUserResult> AddNewUserAsync(string login, string passwordHash, string role, string fullName);
    }
}