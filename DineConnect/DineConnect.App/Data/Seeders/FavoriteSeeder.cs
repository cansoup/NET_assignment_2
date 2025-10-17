using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;

public class FavoriteSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Favorites.AnyAsync())
        {
            db.Favorites.AddRange(
                new Favorite { UserId = 10000001, RestaurantId = 20000002 },
                new Favorite { UserId = 10000002, RestaurantId = 20000001 }
            );

            await db.SaveChangesAsync();
        }
    }
}
