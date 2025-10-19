using Microsoft.EntityFrameworkCore;

namespace DineConnect.App.Util
{
    public interface IIdentifiable
    {
        int Id { get; set; }
    }

    /// <summary>
    /// Provides utility methods for generating the next unique integer IDs for various entities
    /// (User, Restaurant, Post, Comment, Reservation) in the DineConnect database.
    /// </summary>
    public static class IdGenerator
    {
        public static async Task<int> GetNextId<T>(DbSet<T> dbSet, int baseValue) where T : class, IIdentifiable
        {
            var maxId = await dbSet.MaxAsync(e => (int?)e.Id) ?? baseValue;
            return maxId + 1;
        }

        public static Task<int> GetNextUserId(DineConnectContext db) =>
            GetNextId(db.Users, 10000000);

        public static Task<int> GetNextRestaurantId(DineConnectContext db) =>
            GetNextId(db.Restaurants, 20000000);

        public static Task<int> GetNextPostId(DineConnectContext db) =>
            GetNextId(db.Posts, 30000000);

        public static Task<int> GetNextCommentId(DineConnectContext db) =>
            GetNextId(db.Comments, 40000000);

        public static Task<int> GetNextReservationId(DineConnectContext db) =>
            GetNextId(db.Reservations, 50000000);
    }
}
