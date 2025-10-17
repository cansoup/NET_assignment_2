using DineConnect.App.Data;
using DineConnect.App.Models;
using Microsoft.EntityFrameworkCore;

namespace DineConnect.App.Services
{
    /// <summary>
    /// Service layer for Favorites. Encapsulates all EF and data access.
    /// </summary>
    public sealed class FavoriteService : IDisposable
    {
        private readonly DineConnectContext _db;
        private bool _disposed;

        public FavoriteService(DineConnectContext? context = null)
        {
            _db = context ?? new DineConnectContext();
        }

        /// <summary>Ensure DB exists and seed if necessary.</summary>
        public async Task EnsureInitializedAsync()
        {
            await DbSeed.EnsureCreatedAndSeedAsync(_db);
        }

        // --- Lightweight DTOs returned to the View (no UI state) ---

        public sealed class RestaurantSuggestion
        {
            public int Id { get; init; }
            public string Name { get; init; } = "";
            public string Address { get; init; } = "";
            public override string ToString() => string.IsNullOrWhiteSpace(Address) ? Name : $"{Name}, {Address}";
        }

        public sealed class RestaurantDto
        {
            public string Name { get; init; } = "";
            public string Address { get; init; } = "";
        }

        public sealed class FavoriteRow
        {
            public int FavoriteId { get; init; }
            public int RestaurantId { get; init; }
            public RestaurantDto Restaurant { get; init; } = new();
            public int Rating { get; init; }

            public static FavoriteRow From(Favorite f) => new()
            {
                FavoriteId = f.Id,
                RestaurantId = f.RestaurantId,
                Restaurant = new RestaurantDto
                {
                    Name = f.Restaurant?.Name ?? "",
                    Address = f.Restaurant?.Address ?? ""
                },
                Rating = f.Rating
            };
        }

        /// <summary>
        /// Search restaurants by free text (name or address).
        /// </summary>
        public async Task<List<RestaurantSuggestion>> SearchRestaurantsAsync(string query, int take = 20, CancellationToken ct = default)
        {
            query = (query ?? "").Trim();
            if (string.IsNullOrWhiteSpace(query)) return new();

            string like = $"%{query}%";
            var q = _db.Restaurants.AsNoTracking();

            try
            {
                q = q.Where(r =>
                    EF.Functions.Like(r.Name, like) ||
                    (r.Address != null && EF.Functions.Like(r.Address, like)));
            }
            catch
            {
                q = q.Where(r =>
                    r.Name.Contains(query) ||
                    (r.Address != null && r.Address.Contains(query)));
            }

            return await q
                .OrderBy(r => r.Name)
                .Take(take)
                .Select(r => new RestaurantSuggestion
                {
                    Id = r.Id,
                    Name = r.Name,
                    Address = r.Address ?? ""
                })
                .ToListAsync(ct);
        }

        /// <summary>
        /// Get all favorites for a user.
        /// </summary>
        public async Task<List<FavoriteRow>> GetFavoritesForUserAsync(int userId, CancellationToken ct = default)
        {
            var items = await _db.Favorites
                .AsNoTracking()
                .Include(f => f.Restaurant)
                .Where(f => f.UserId == userId)
                .OrderBy(f => f.Restaurant!.Name)
                .ToListAsync(ct);

            return items.Select(FavoriteRow.From).ToList();
        }

        public sealed record AddFavoriteResult(bool Ok, string? Error, FavoriteRow? Created);

        /// <summary>
        /// Adds (or links) a favorite for a user. Creates the Restaurant if it doesn't exist (name+address tuple).
        /// </summary>
        public async Task<AddFavoriteResult> AddFavoriteAsync(int userId, string restaurantName, string restaurantAddress, int rating, CancellationToken ct = default)
        {
            restaurantName = (restaurantName ?? "").Trim();
            restaurantAddress = (restaurantAddress ?? "").Trim();

            if (string.IsNullOrWhiteSpace(restaurantName))
                return new(false, "Restaurant name is required.", null);

            if (rating < 1 || rating > 5)
                return new(false, "Rating must be between 1 and 5.", null);

            try
            {
                // 1) Find or create the restaurant
                var existingRestaurant = await _db.Restaurants
                    .FirstOrDefaultAsync(r => r.Name == restaurantName && r.Address == restaurantAddress, ct);

                if (existingRestaurant is null)
                {
                    existingRestaurant = new Restaurant
                    {
                        Name = restaurantName,
                        Address = restaurantAddress
                    };
                    await _db.Restaurants.AddAsync(existingRestaurant, ct);
                    await _db.SaveChangesAsync(ct);
                }

                // 2) Check if already favorited by user
                bool already = await _db.Favorites
                    .AnyAsync(f => f.UserId == userId && f.RestaurantId == existingRestaurant.Id, ct);

                if (already)
                    return new(false, "This restaurant is already in your favorites.", null);

                // 3) Create favorite
                var favorite = new Favorite
                {
                    UserId = userId,
                    RestaurantId = existingRestaurant.Id,
                    Rating = rating
                };

                await _db.Favorites.AddAsync(favorite, ct);
                await _db.SaveChangesAsync(ct);

                // 4) Return DTO with nested Restaurant object to match XAML
                var row = new FavoriteRow
                {
                    FavoriteId = favorite.Id,
                    RestaurantId = existingRestaurant.Id,
                    Restaurant = new RestaurantDto
                    {
                        Name = existingRestaurant.Name,
                        Address = existingRestaurant.Address ?? ""
                    },
                    Rating = favorite.Rating
                };

                return new(true, null, row);
            }
            catch (DbUpdateException ex)
            {
                return new(false, ex.GetBaseException().Message, null);
            }
            catch (Exception ex)
            {
                return new(false, ex.Message, null);
            }
        }

        public sealed record DeleteFavoriteResult(bool Ok, string? Error);

        public async Task<DeleteFavoriteResult> DeleteFavoriteAsync(int userId, int favoriteId, CancellationToken ct = default)
        {
            try
            {
                // 1) Load the favorite, checking ownership
                var favorite = await _db.Favorites
                    .FirstOrDefaultAsync(f => f.Id == favoriteId && f.UserId == userId, ct);

                if (favorite is null)
                    return new(false, "Favorite not found or does not belong to this user.");

                // 2) Remove it
                _db.Favorites.Remove(favorite);
                await _db.SaveChangesAsync(ct);

                return new(true, null);
            }
            catch (Exception ex)
            {
                return new(false, ex.GetBaseException().Message);
            }
        }


        public void Dispose()
        {
            if (_disposed) return;
            _db.Dispose();
            _disposed = true;
        }
    }
}
