using System.Linq;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.UI.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string login, string password);
    }


    public class AuthService : IAuthService
    {
        private readonly AccountingDbContext _db;


        public AuthService(AccountingDbContext db)
        {
            _db = db;
        }


        public async Task<User?> AuthenticateAsync(string login, string password)
        {
            // NOTE: For demo purposes we compare plaintext password hash using PasswordHash field.
            // In production use salted hashing (PBKDF2/Argon2) and secure verification.
            var user = _db.Users.FirstOrDefault(u => u.Login == login);
            if (user == null) return null;
            if (user.PasswordHash == password) return user;
            return null;
        }
    }
}
