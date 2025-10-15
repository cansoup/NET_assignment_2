using DineConnect.App.Data;
using System.Linq;
using System.Windows;

namespace DineConnect.App
{
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            using var db = new DineConnectContext();
            await DbSeed.EnsureCreatedAndSeedAsync(db);
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