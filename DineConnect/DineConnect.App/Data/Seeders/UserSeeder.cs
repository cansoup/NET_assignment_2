using DineConnect.App.Data.Seeders;
using Microsoft.EntityFrameworkCore;

public class UserSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            var aliceHash = BCrypt.Net.BCrypt.HashPassword("hash123");
            var bobHash = BCrypt.Net.BCrypt.HashPassword("hash456");

            db.Users.AddRange(
                new DineConnect.App.Models.User { Id = 10000001, UserName = "alice", PasswordHash = aliceHash },
                new DineConnect.App.Models.User { Id = 10000002, UserName = "bob", PasswordHash = bobHash }
            );

            await db.SaveChangesAsync();
        }
    }
}
