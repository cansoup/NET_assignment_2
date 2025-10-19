using DineConnect.App.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineConnect.App.Models
{
    /// <summary>
    /// Represents a post in the community feed, created by a user.
    /// </summary>
    public class Post : IIdentifiable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
