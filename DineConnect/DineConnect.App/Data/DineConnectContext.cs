using DineConnect.App.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the Entity Framework database context for the DineConnect application,
/// defining the database sets and relationships between entities.
/// </summary>
public class DineConnectContext : DbContext
{
    public DineConnectContext() { }

    public DineConnectContext(DbContextOptions<DineConnectContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            _ = options.UseSqlite("Data Source=dineconnect.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        model.Entity<Favorite>()
            .HasKey(f => new { f.UserId, f.RestaurantId });

        model.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId);

        model.Entity<Favorite>()
            .HasOne(f => f.Restaurant)
            .WithMany(r => r.FavoriteByUsers)
            .HasForeignKey(f => f.RestaurantId);
    }
}
