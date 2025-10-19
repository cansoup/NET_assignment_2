using DineConnect.App.Models;
using DineConnect.App.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DineConnect.App.Services
{
    /// <summary>
    /// Service layer for reservations. Encapsulates all EF and data access.
    /// </summary>
    public sealed class ReservationsService : IDisposable, IInitializableService
    {
        private readonly DineConnectContext _db;
        private bool _disposed;

        public ReservationsService(DineConnectContext? context = null)
        {
            _db = context ?? new DineConnectContext();
        }

        /// <summary>
        /// Ensure DB exists and seed if necessary.
        /// </summary>
        public async Task EnsureInitializedAsync()
        {
            await DbSeed.EnsureCreatedAndSeedAsync(_db);
        }

        /// <summary>
        /// Lightweight item for restaurant dropdown binding.
        /// </summary>
        public sealed class RestaurantItem
        {
            public int Id { get; init; }
            public string Name { get; init; } = "";
            public override string ToString() => Name;
        }

        /// <summary>
        /// Lightweight row for reservations grid binding.
        /// </summary>
        public sealed class ReservationRow
        {
            public int Id { get; init; }
            public int RestaurantId { get; init; }
            public string RestaurantName { get; init; } = "";
            public int UserId { get; init; }
            public DateTime At { get; init; }
            public int PartySize { get; init; }
            public ReservationStatus Status { get; init; }

            public static ReservationRow From(Reservation r, string restaurantName) => new()
            {
                Id = r.Id,
                RestaurantId = r.RestaurantId,
                RestaurantName = restaurantName,
                UserId = r.UserId,
                At = r.At,
                PartySize = r.PartySize,
                Status = r.Status
            };
        }

        public async Task<List<RestaurantItem>> GetRestaurantsAsync(CancellationToken ct = default)
        {
            return await _db.Restaurants
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new RestaurantItem { Id = r.Id, Name = r.Name })
                .ToListAsync(ct);
        }

        public async Task<List<ReservationRow>> GetReservationsForUserAsync(int userId, CancellationToken ct = default)
        {
            // Preload restaurant names to avoid N+1
            var restaurants = await _db.Restaurants.AsNoTracking()
                .Select(r => new { r.Id, r.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

            var reservations = await _db.Reservations
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.At)
                .ToListAsync(ct);

            return reservations
                .Select(r => ReservationRow.From(r, restaurants.TryGetValue(r.RestaurantId, out var n) ? n : $"#{r.RestaurantId}"))
                .ToList();
        }

        public async Task<List<ReservationRow>> GetReservationsForUserByStatusAsync(int userId, ReservationStatus status, CancellationToken ct = default)
        {
            var restaurants = await _db.Restaurants.AsNoTracking()
                .Select(r => new { r.Id, r.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

            var reservations = await _db.Reservations
                .AsNoTracking()
                .Where(r => r.UserId == userId && r.Status == status)
                .OrderBy(r => r.At)
                .ToListAsync(ct);

            return reservations
                .Select(r => ReservationRow.From(r, restaurants.TryGetValue(r.RestaurantId, out var n) ? n : $"#{r.RestaurantId}"))
                .ToList();
        }

        public sealed record CreateReservationResult(bool Ok, string? Error, ReservationRow? Created);

        public async Task<CreateReservationResult> CreateReservationAsync(
            int userId, int restaurantId, DateTime at, int partySize, CancellationToken ct = default)
        {
            if (partySize < 1) return new(false, "Party size must be at least 1.", null);

            // Verify restaurant exists
            var restaurant = await _db.Restaurants.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == restaurantId, ct);
            if (restaurant is null) return new(false, $"Restaurant #{restaurantId} not found.", null);

            var entity = new Reservation
            {
                RestaurantId = restaurantId,
                UserId = userId,
                At = at,
                PartySize = partySize,
                Status = ReservationStatus.Confirmed
            };

            try
            {
                await _db.Reservations.AddAsync(entity, ct);
                await _db.SaveChangesAsync(ct);

                var row = ReservationRow.From(entity, restaurant.Name);
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

        public sealed record DeleteReservationResult(bool Ok, string? Error);

        public async Task<DeleteReservationResult> DeleteReservationAsync(int userId, int reservationId, CancellationToken ct = default)
        {
            var entity = await _db.Reservations.FindAsync(new object?[] { reservationId }, ct);
            if (entity is null)
            {
                return new(true, null);
            }

            if (entity.UserId != userId)
            {
                return new(false, "You can only delete your own reservations.");
            }

            try
            {
                _db.Reservations.Remove(entity);
                await _db.SaveChangesAsync(ct);
                return new(true, null);
            }
            catch (DbUpdateException ex)
            {
                return new(false, ex.GetBaseException().Message);
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
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
