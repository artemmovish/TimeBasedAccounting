using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.Core.Interfaces;

namespace TimeBasedAccounting.Core.Services
{
    public class UserService : IUserService
    {
        private readonly AccountingDbContext _context;

        public UserService(AccountingDbContext context)
        {
            _context = context;
        }

        public async Task<AddUserResult> AddNewUserAsync(string login, string passwordHash, string role, string fullName)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_login", login),
                new MySqlParameter("@p_password_hash", passwordHash),
                new MySqlParameter("@p_role", role),
                new MySqlParameter("@p_fullname", fullName)
            };
            
            // Важно: используем контекстное DbSet для AddUserResult
            var result = await _context.AddUserResults
                .FromSqlRaw("CALL AddNewUser(@p_login, @p_password_hash, @p_role, @p_fullname)", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return result ?? new AddUserResult { Status = "ERROR", Message = "Unknown error" };
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
