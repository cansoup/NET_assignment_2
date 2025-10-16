using System.Collections.Generic;

namespace DineConnect.App.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string? Phone { get; set; }

        public override string ToString() => Name;

        // Navigation
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
       // naviation property
        public virtual ICollection<Favorite> FavoriteByUsers { get; set; } = new HashSet<Favorite>();
    }
}
