using Microsoft.EntityFrameworkCore;
using DineConnect.App.Services.Validation;

namespace DineConnect.App.Services
{
    public class AuthService
    {
        public async Task<bool> LoginAsync(string username, string password)
        {
            await using var dbContext = new DineConnectContext();

            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == username.Trim());

            var v = ValidateUser.ValidateLoginInput(username, password, user?.PasswordHash);

            if (!v.IsValid || user == null) return false;

            AppState.CurrentUser = user;
            return true;
        }


        public async Task<bool> RegisterAsync(string username, string password)
        {
            var v = ValidateUser.ValidateRegistrationInput(username, password);
            if (!v.IsValid) return false;

            await using var dbContext = new DineConnectContext();

            var normalizedUsername = username.Trim();

            // Check if username is already taken (DB concern stays here)
            bool exists = await dbContext.Users
                .AnyAsync(u => u.UserName == normalizedUsername);

            if (exists) return false; // Username already exists

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new Models.User
            {
                UserName = normalizedUsername,
                PasswordHash = passwordHash
            };

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
