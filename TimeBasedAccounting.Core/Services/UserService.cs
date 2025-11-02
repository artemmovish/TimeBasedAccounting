using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Services
{
    public class UserService : IUserService
    {
        private readonly AccountingDbContext _context;

        public UserService(AccountingDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddNewUserAsync(string login, string passwordHash, string fullName)
        {
            // Проверяем, существует ли уже пользователь с таким логином
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Пользователь с таким логином уже существует");
            }

            // Создаем нового пользователя
            var newUser = new User
            {
                Login = login.Trim(),
                PasswordHash = passwordHash,
                Role = UserRole.Accountant,
                FullName = fullName.Trim(),
                CreatedAt = DateTime.Now
            };

            // Добавляем пользователя в базу данных
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Возвращаем созданного пользователя
            return newUser;
        }

        public async Task<LoginResult> UserLoginAsync(string login, string password)
    {
        await using var connection = new MySqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new MySqlCommand("CALL UserLogin(@p_login, @p_password)", connection);
        command.Parameters.AddWithValue("@p_login", login);
        command.Parameters.AddWithValue("@p_password", password);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new LoginResult
            {
                Status = reader.GetString("Status"),
                UserId = reader.IsDBNull("UserId") ? null : reader.GetInt32("UserId"),
                Role = reader.IsDBNull("Role") ? null : reader.GetString("Role"),
                FullName = reader.IsDBNull("FullName") ? null : reader.GetString("FullName")
            };
        }

        return new LoginResult { Status = "FAILED" };
    }
    }


public class AddUserResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
    }

    public class LoginResult
    {
        public string Status { get; set; }
        public int? UserId { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
    }
}
