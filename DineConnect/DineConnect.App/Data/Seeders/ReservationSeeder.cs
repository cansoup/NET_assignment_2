using DineConnect.App.Data.Seeders;
using DineConnect.App.Models;
using DineConnect.App.Util;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Seeds initial reservation data into the database if no reservations exist.
/// </summary>
public class ReservationSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Reservations.AnyAsync())
        {
            var reservationId1 = await IdGenerator.GetNextReservationId(db);
            var reservationId2 = reservationId1 + 1;

            db.Reservations.AddRange(
                new Reservation
                {
                    Id = reservationId1,
                    RestaurantId = 20000001,
                    UserId = 10000001,
                    At = DateTime.Now.AddDays(1),
                    PartySize = 2,
                    Status = ReservationStatus.Confirmed
                },
                new Reservation
                {
                    Id = reservationId2,
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
