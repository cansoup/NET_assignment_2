using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;

/// <summary>
/// Seeds initial restaurant data into the database if no restaurants exist.
/// </summary>
public class RestaurantSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Restaurants.AnyAsync())
        {
            db.Restaurants.AddRange(
                new Restaurant
                {
                    Id = 20000001,
                    Name = "Ocean View Grill",
                    Address = "123 Beach Ave",
                    Lat = 35.556,
                    Lng = -120.678,
                    Phone = "555-1234"
                },
                new Restaurant
                {
                    Id = 20000002,
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
