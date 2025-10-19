using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;
using DineConnect.App.Util;

/// <summary>
/// Seeds initial restaurant data into the database if no restaurants exist.
/// </summary>
public class RestaurantSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Restaurants.AnyAsync())
        {
            var restaurantId1 = await IdGenerator.GetNextRestaurantId(db);
            var restaurantId2 = restaurantId1 + 1;

            db.Restaurants.AddRange(
                new Restaurant
                {
                    Id = restaurantId1,
                    Name = "Ocean View Grill",
                    Address = "123 Beach Ave",
                    Lat = 35.556,
                    Lng = -120.678,
                    Phone = "555-1234"
                },
                new Restaurant
                {
                    Id = restaurantId2,
                    Name = "Mountain Top Diner",
                    Address = "789 Hill Road",
                    Lat = 40.123,
                    Lng = -105.456,
                    Phone = "555-9876"
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
