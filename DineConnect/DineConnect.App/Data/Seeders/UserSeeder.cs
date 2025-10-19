using DineConnect.App.Data.Seeders;
using DineConnect.App.Util;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Seeds initial user data into the database if no users exist.
/// </summary>
public class UserSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            var aliceId = await IdGenerator.GetNextUserId(db);
            var bobId = aliceId + 1;

            var aliceHash = BCrypt.Net.BCrypt.HashPassword("hash123");
            var bobHash = BCrypt.Net.BCrypt.HashPassword("hash456");

            db.Users.AddRange(
                new DineConnect.App.Models.User { Id = aliceId, UserName = "alice", PasswordHash = aliceHash },
                new DineConnect.App.Models.User { Id = bobId, UserName = "bob", PasswordHash = bobHash }
            );

            await db.SaveChangesAsync();
        }
    }
}
