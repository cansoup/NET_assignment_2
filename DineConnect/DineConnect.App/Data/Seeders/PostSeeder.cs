using DineConnect.App.Data.Seeders;
using DineConnect.App.Models;
using DineConnect.App.Util;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Seeds initial post data into the database if no posts exist.
/// </summary>
public class PostSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Posts.AnyAsync())
        {
            var postId1 = await IdGenerator.GetNextPostId(db);
            var postId2 = postId1 + 1;

            db.Posts.AddRange(
                new Post
                {
                    Id = postId1,
                    UserId = 10000001,
                    Title = "Amazing Seafood!",
                    Content = "Had a great time at Ocean View Grill!"
                },
                new Post
                {
                    Id = postId2,
                    UserId = 10000002,
                    Title = "Love this place",
                    Content = "Mountain Top Diner has the coziest vibe!"
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
