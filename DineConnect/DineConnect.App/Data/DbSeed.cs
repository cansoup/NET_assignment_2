// DineConnect.Infrastructure/DbSeed.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DineConnect.App.Data
{
    public static class DbSeed
    {
        public static async Task EnsureCreatedAndSeedAsync(DineConnectContext db)
        {
            await db.Database.EnsureCreatedAsync();

            // Users
            if (!await db.Users.AnyAsync())
            {
                var aliceHash = BCrypt.Net.BCrypt.HashPassword("hash123");
                var bobHash = BCrypt.Net.BCrypt.HashPassword("hash456");

                db.Users.AddRange(
                    new Models.User { Id = 10000001, UserName = "alice", PasswordHash = aliceHash },
                    new Models.User { Id = 10000002, UserName = "bob", PasswordHash = bobHash }
                );
                await db.SaveChangesAsync();
            }

            // Restaurants
            if (!await db.Restaurants.AnyAsync())
            {
                db.Restaurants.AddRange(
                    new Models.Restaurant
                    {
                        Id = 20000001,
                        Name = "Ocean View Grill",
                        Address = "123 Beach Ave",
                        Lat = 35.556,
                        Lng = -120.678,
                        Phone = "555-1234"
                    },
                    new Models.Restaurant
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

            // Posts
            if (!await db.Posts.AnyAsync())
            {
                db.Posts.AddRange(
                    new Models.Post
                    {
                        Id = 30000001,
                        UserId = 10000001, // alice
                        Title = "Amazing Seafood!",
                        Content = "Had a great time at Ocean View Grill!"
                    },
                    new Models.Post
                    {
                        Id = 30000002,
                        UserId = 10000002, // bob
                        Title = "Love this place",
                        Content = "Mountain Top Diner has the coziest vibe!"
                    }
                );
                await db.SaveChangesAsync();
            }

            // Comments
            if (!await db.Comments.AnyAsync())
            {
                db.Comments.AddRange(
                    new Models.Comment
                    {
                        Id = 40000001,
                        UserId = 10000002, // bob on alice’s post
                        PostId = 30000001,
                        Content = "Totally agree! Their shrimp is the best."
                    },
                    new Models.Comment
                    {
                        Id = 40000002,
                        UserId = 10000001, // alice comments back
                        PostId = 30000002,
                        Content = "Thanks! I need to try that diner too."
                    }
                );
                await db.SaveChangesAsync();
            }

            // Reservations
            if (!await db.Reservations.AnyAsync())
            {
                db.Reservations.AddRange(
                    new Models.Reservation
                    {
                        Id = 50000001,
                        RestaurantId = 20000001,
                        UserId = 10000001,
                        At = DateTime.Now.AddDays(1),
                        PartySize = 2,
                        Status = Models.ReservationStatus.Confirmed
                    },
                    new Models.Reservation
                    {
                        Id = 50000002,
                        RestaurantId = 20000002,
                        UserId = 10000002,
                        At = DateTime.Now.AddDays(2),
                        PartySize = 4,
                        Status = Models.ReservationStatus.Pending
                    }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
