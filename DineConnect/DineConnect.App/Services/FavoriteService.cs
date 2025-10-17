using DineConnect.App.Models;
using Microsoft.EntityFrameworkCore;

namespace DineConnect.App.Services
{
    public class FavoriteService
    {
        private readonly DineConnectContext _db;

        public FavoriteService(DineConnectContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<List<Favorite>> GetFavoritesForUserAsync(int userId)
        {
            return await _db.Favorites
                .Include(f => f.Restaurant)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> AddFavoriteAsync(int userId, string restaurantName, string restaurantAddress, int rating)
        {
            // 1. Check if the restaurant already exists in our database
            var existingRestaurant = await _db.Restaurants
                .FirstOrDefaultAsync(r => r.Name == restaurantName && r.Address == restaurantAddress);

            // 2. if it doesn't exist, creat it
            if (existingRestaurant == null)
            {
                existingRestaurant = new Restaurant
                {
                    Name = restaurantName,
                    Address = restaurantAddress,
                };
                _db.Restaurants.Add(existingRestaurant);
                await _db.SaveChangesAsync();
            }

            // 3. check if this user has already favorited this restaurant
            bool alreadyFavorited = await _db.Favorites
                .AnyAsync(f => f.UserId == userId && f.RestaurantId == existingRestaurant.Id);

            if (alreadyFavorited)
            {
                // Already favorited, no action needed
                return false;
            }

            // 4. create the link in the favorites table
            var newFavorite = new Favorite
            {
                UserId = userId,
                RestaurantId = existingRestaurant.Id,
                Rating = rating
            };
            _db.Favorites.Add(newFavorite);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
