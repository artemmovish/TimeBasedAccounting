using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Services
{
    public class UserService : IUserService
    {
        private readonly AccountingDbContext _db;

        public UserService(AccountingDbContext db) => _db = db;

        public async Task<User?> AuthenticateAsync(string login, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null) return null;

            if (user.PasswordHash != password) return null;

            return user;
        }

        public Task<User> GetUserByIdAsync(int userId) =>
            _db.Users.FirstAsync(u => u.UserId == userId);

        public Task<IEnumerable<User>> GetAllUsersAsync() =>
            _db.Users.ToListAsync().ContinueWith(t => t.Result.AsEnumerable());
    }
}
