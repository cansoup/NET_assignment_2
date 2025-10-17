using DineConnect.App.Data.Seeders;

public static class DbSeed
{
    public static async Task EnsureCreatedAndSeedAsync(DineConnectContext db)
    {
        await db.Database.EnsureCreatedAsync();

        var seeders = new List<ISeeder>
        {
            new UserSeeder(),
            new RestaurantSeeder(),
            new PostSeeder(),
            new CommentSeeder(),
            new ReservationSeeder(),
            new FavoriteSeeder()
        };

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(db);
        }
    }
}
