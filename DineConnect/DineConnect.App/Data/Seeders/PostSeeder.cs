using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;

/// <summary>
/// Seeds initial post data into the database if no posts exist.
/// </summary>
public class PostSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Posts.AnyAsync())
        {
            db.Posts.AddRange(
                new Post
                {
                    Id = 30000001,
                    UserId = 10000001,
                    Title = "Amazing Seafood!",
                    Content = "Had a great time at Ocean View Grill!"
                },
                new Post
                {
                    Id = 30000002,
                    UserId = 10000002,
                    Title = "Love this place",
                    Content = "Mountain Top Diner has the coziest vibe!"
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
