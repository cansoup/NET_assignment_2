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

            if (!db.Restaurants.Any())
            {
                db.Restaurants.Add(new Models.Restaurant
                {
                    Name = "Ocean View Grill",
                    Address = "123 Beach Ave",
                    Lat = 35.556,
                    Lng = -120.678,
                    Phone = "555-1234"
                });
                db.SaveChanges();
            }

            // To test if the data was added, uncomment below:
            /*
            var all = db.Restaurants.ToList();
            foreach (var r in all)
                System.Diagnostics.Debug.WriteLine($"[DB] {r.Id}: {r.Name} - {r.Address}");
            */
        }
    }
}
