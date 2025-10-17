using DineConnect.App.Data;
using DineConnect.App.Services;
using DineConnect.App.Services.Interfaces;
using DineConnect.App.Services.Validation;
using Microsoft.EntityFrameworkCore;

public class AuthService : IInitializableService
{
    public async Task EnsureInitializedAsync()
    {
        await using var dbContext = new DineConnectContext();
        await DbSeed.EnsureCreatedAndSeedAsync(dbContext);
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        await EnsureInitializedAsync();

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
        await EnsureInitializedAsync();

        var v = ValidateUser.ValidateRegistrationInput(username, password);
        if (!v.IsValid) return false;

        await using var dbContext = new DineConnectContext();

        var normalizedUsername = username.Trim();

        bool exists = await dbContext.Users
            .AnyAsync(u => u.UserName == normalizedUsername);

        if (exists) return false;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var newUser = new DineConnect.App.Models.User
        {
            UserName = normalizedUsername,
            PasswordHash = passwordHash
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
