using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;

public class ReservationSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Reservations.AnyAsync())
        {
            db.Reservations.AddRange(
                new Reservation
                {
                    Id = 50000001,
                    RestaurantId = 20000001,
                    UserId = 10000001,
                    At = DateTime.Now.AddDays(1),
                    PartySize = 2,
                    Status = ReservationStatus.Confirmed
                },
                new Reservation
                {
                    Id = 50000002,
                    RestaurantId = 20000002,
                    UserId = 10000002,
                    At = DateTime.Now.AddDays(2),
                    PartySize = 4,
                    Status = ReservationStatus.Pending
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
