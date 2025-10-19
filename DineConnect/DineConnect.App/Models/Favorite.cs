namespace DineConnect.App.Models
{
    /// <summary>
    /// Represents a user's favorite restaurant entry, including rating and navigation properties.
    /// </summary>
    public class Favorite
    {
        public int Id { get; set; }

        // Foreign Keys to link User and Restaurant
        public int UserId { get; set; }
        public int RestaurantId { get; set; }

        public int Rating { get; set; }

        // Navigation properties to the actual objects
        public User User { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
