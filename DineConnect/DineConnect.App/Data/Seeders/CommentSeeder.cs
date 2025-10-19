using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;

/// <summary>
/// Seeds initial comment data into the database if no comments exist.
/// </summary>
public class CommentSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Comments.AnyAsync())
        {
            db.Comments.AddRange(
                new Comment
                {
                    Id = 40000001,
                    UserId = 10000002,
                    PostId = 30000001,
                    Content = "Totally agree! Their shrimp is the best."
                },
                new Comment
                {
                    Id = 40000002,
                    UserId = 10000001,
                    PostId = 30000002,
                    Content = "Thanks! I need to try that diner too."
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
