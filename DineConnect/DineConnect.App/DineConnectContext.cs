using DineConnect.App.Models;
using Microsoft.EntityFrameworkCore;

public class DineConnectContext : DbContext
{
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // SQLite:
        options.UseSqlite("Data Source=dineconnect.db");

        // OR for SQL Server (comment SQLite, uncomment below):
        // options.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=DineConnect;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        // optional: relationships, constraints, seed data, etc.
    }
}
