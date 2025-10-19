using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;

/// <summary>
/// Seeds initial favorite data into the database if no favorites exist.
/// </summary>
public class FavoriteSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Favorites.AnyAsync())
        {
            db.Favorites.AddRange(
                new Favorite { UserId = 10000001, RestaurantId = 20000002, Rating = 5 },
                new Favorite { UserId = 10000002, RestaurantId = 20000001, Rating = 3 }
            );

            await db.SaveChangesAsync();
        }
    }
}
