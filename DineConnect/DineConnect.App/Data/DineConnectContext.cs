using DineConnect.App.Models;
using Microsoft.EntityFrameworkCore;

public class DineConnectContext : DbContext
{
    public DineConnectContext() { }

    // Added so tests can inject a fake SQL database (like in-memory SQLite)
    public DineConnectContext(DbContextOptions<DineConnectContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Only apply default connection if no options were passed (so tests can override!)
        if (!options.IsConfigured)
        {
            // SQLite:
            options.UseSqlite("Data Source=dineconnect.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        // optional: relationships, constraints, seed data, etc.
    }
}
