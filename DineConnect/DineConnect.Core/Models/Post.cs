using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineConnect.Core.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UsertId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
