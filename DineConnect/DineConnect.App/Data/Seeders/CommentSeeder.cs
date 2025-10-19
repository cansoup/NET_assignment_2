using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;
using DineConnect.App.Util;

/// <summary>
/// Seeds initial comment data into the database if no comments exist.
/// </summary>
public class CommentSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Comments.AnyAsync())
        {
            var commentId1 = await IdGenerator.GetNextCommentId(db);
            var commentId2 = commentId1 + 1;

            db.Comments.AddRange(
                new Comment
                {
                    Id = commentId1,
                    UserId = 10000002,
                    PostId = 30000001,
                    Content = "Totally agree! Their shrimp is the best."
                },
                new Comment
                {
                    Id = commentId2,
                    UserId = 10000001,
                    PostId = 30000002,
                    Content = "Thanks! I need to try that diner too."
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
