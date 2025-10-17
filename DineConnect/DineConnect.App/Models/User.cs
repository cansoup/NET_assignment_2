using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineConnect.App.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        // This collection links a user to their favorite entries
        public virtual ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>(); 

    }
}
