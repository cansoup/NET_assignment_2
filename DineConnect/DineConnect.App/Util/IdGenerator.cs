using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DineConnect.App.Util
{
    public static class IdGenerator
    {
        public static async Task<int> GetNextUserIdAsync(DineConnectContext db)
        {
            var maxId = await db.Users.MaxAsync(u => (int?)u.Id) ?? 10000000;
            return maxId + 1;
        }

        public static async Task<int> GetNextRestaurantIdAsync(DineConnectContext db)
        {
            var maxId = await db.Restaurants.MaxAsync(r => (int?)r.Id) ?? 20000000;
            return maxId + 1;
        }

        public static async Task<int> GetNextPostIdAsync(DineConnectContext db)
        {
            var maxId = await db.Posts.MaxAsync(p => (int?)p.Id) ?? 30000000;
            return maxId + 1;
        }

        public static async Task<int> GetNextCommentIdAsync(DineConnectContext db)
        {
            var maxId = await db.Comments.MaxAsync(c => (int?)c.Id) ?? 40000000;
            return maxId + 1;
        }

        public static async Task<int> GetNextReservationIdAsync(DineConnectContext db)
        {
            var maxId = await db.Reservations.MaxAsync(r => (int?)r.Id) ?? 50000000;
            return maxId + 1;
        }
    }
}
