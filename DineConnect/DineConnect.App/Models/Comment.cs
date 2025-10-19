using DineConnect.App.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineConnect.App.Models
{
    /// <summary>
    /// Represents a comment made by a user on a post in the community feed.
    /// </summary>
    public class Comment : IIdentifiable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
    }
}
