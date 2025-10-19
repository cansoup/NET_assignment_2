using DineConnect.App.Data.Seeders;

/// <summary>
/// Provides database initialization and seeding for the DineConnect application.
/// </summary>
public static class DbSeed
{
    public static async Task EnsureCreatedAndSeedAsync(DineConnectContext db)
    {
        _ = await db.Database.EnsureCreatedAsync();

        List<ISeeder> seeders =
        [
            new UserSeeder(),
            new RestaurantSeeder(),
            new PostSeeder(),
            new CommentSeeder(),
            new ReservationSeeder(),
            new FavoriteSeeder()
        ];

        foreach (ISeeder seeder in seeders)
        {
            await seeder.SeedAsync(db);
        }
    }
}
