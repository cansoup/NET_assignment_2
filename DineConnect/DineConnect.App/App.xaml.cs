using System.Linq;
using System.Windows;

namespace DineConnect.App
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            using var db = new DineConnectContext();
            db.Database.EnsureCreated();

            // Seed Users
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new Models.User { Id = 10000001, UserName = "alice", PasswordHash = "hash123" },
                    new Models.User { Id = 10000002, UserName = "bob", PasswordHash = "hash456" }
                );
                db.SaveChanges();
            }

            // Seed Restaurants
            if (!db.Restaurants.Any())
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
                db.SaveChanges();
            }

            // Seed Posts
            if (!db.Posts.Any())
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
                db.SaveChanges();
            }

            // Seed Comments
            if (!db.Comments.Any())
            {
                db.Comments.AddRange(
                    new Models.Comment
                    {
                        Id = 40000001,
                        UserId = 10000002, // bob comments on alice’s post
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
                db.SaveChanges();
            }

            // Seed Reservations
            if (!db.Reservations.Any())
            {
                db.Reservations.AddRange(
                    new Models.Reservation
                    {
                        Id = 50000001,
                        RestaurantId = 20000001,
                        UserId = 10000001, // alice
                        At = DateTime.Now.AddDays(1),
                        PartySize = 2,
                        Status = Models.ReservationStatus.Confirmed
                    },
                    new Models.Reservation
                    {
                        Id = 50000002,
                        RestaurantId = 20000002,
                        UserId = 10000002, // bob
                        At = DateTime.Now.AddDays(2),
                        PartySize = 4,
                        Status = Models.ReservationStatus.Pending
                    }
                );
                db.SaveChanges();
            }

            // Uncomment below to see if data was added:
            /*
            System.Diagnostics.Debug.WriteLine("========= DEBUG DB SEED REPORT =========");

            var userCount = db.Users.Count();
            System.Diagnostics.Debug.WriteLine($"Users ({userCount}):");
            db.Users.ToList().ForEach(u =>
                System.Diagnostics.Debug.WriteLine($" -> [User] {u.Id} | {u.UserName}")
            );

            var restaurantCount = db.Restaurants.Count();
            System.Diagnostics.Debug.WriteLine($"Restaurants ({restaurantCount}):");
            db.Restaurants.ToList().ForEach(r =>
                System.Diagnostics.Debug.WriteLine($" -> [Restaurant] {r.Id} | {r.Name} | {r.Address}")
            );

            var postCount = db.Posts.Count();
            System.Diagnostics.Debug.WriteLine($"Posts ({postCount}):");
            db.Posts.ToList().ForEach(p =>
                System.Diagnostics.Debug.WriteLine($" -> [Post] {p.Id} | {p.Title} | From User {p.UserId}")
            );

            var commentCount = db.Comments.Count();
            System.Diagnostics.Debug.WriteLine($"Comments ({commentCount}):");
            db.Comments.ToList().ForEach(c =>
                System.Diagnostics.Debug.WriteLine($" -> [Comment] {c.Id} | On Post {c.PostId} | By User {c.UserId}")
            );

            var reservationCount = db.Reservations.Count();
            System.Diagnostics.Debug.WriteLine($"Reservations ({reservationCount}):");
            db.Reservations.ToList().ForEach(rv =>
                System.Diagnostics.Debug.WriteLine($" -> [Reservation] {rv.Id} | Restaurant {rv.RestaurantId} | User {rv.UserId} | {rv.At} | {rv.Status}")
            );

            System.Diagnostics.Debug.WriteLine("=========================================");
            */

        }

    }
}