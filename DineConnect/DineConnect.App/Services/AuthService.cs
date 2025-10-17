
using Microsoft.EntityFrameworkCore;

namespace DineConnect.App.Services
{
    public class AuthService
    {
        public async Task<bool> LoginAsync(string username, string password)
        {
            await using var dbContext = new DineConnectContext();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null) return false; // User not found

            // Verify the provided password against the sotred hash
            bool verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (verified)
            {
                AppState.CurrentUser = user;
                return true;
            }
            else
            {
                return false; // Password mismatch
            }
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            await using var dbContext = new DineConnectContext();

            // Check if username is already taken
            if (await dbContext.Users.AnyAsync(u => u.UserName == username))
            {
                return false; // Username already exists
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new Models.User
            {
                UserName = username,
                PasswordHash = passwordHash
            };

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
