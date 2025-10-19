using DineConnect.App.Util;

namespace DineConnect.App.Models
{
    /// <summary>
    /// Represents a user of the DineConnect application.
    /// </summary>
    public class User : IIdentifiable
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        // This collection links a user to their favorite entries
        public virtual ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();

    }
}
