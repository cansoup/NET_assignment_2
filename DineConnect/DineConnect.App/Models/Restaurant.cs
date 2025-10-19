using DineConnect.App.Util;
using System.Collections.Generic;

namespace DineConnect.App.Models
{
    /// <summary>
    /// Represents a restaurant, including its location, contact information, and related reservations and favorites.
    /// </summary>
    public class Restaurant : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string? Phone { get; set; }

        public virtual ICollection<Favorite> FavoriteByUsers { get; set; } = new HashSet<Favorite>();
    }
}
