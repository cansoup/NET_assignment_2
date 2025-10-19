using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using DineConnect.App.Data;
using DineConnect.App.Models;

namespace DineConnect.Tests
{
    // Reusable helpers for all fixtures
    internal static class TestDb
    {
        /// <summary>
        /// Creates an in-memory SQLite database context for testing.
        /// </summary>
        public static (DineConnectContext Ctx, SqliteConnection Conn) CreateInMemory()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();

            var options = new DbContextOptionsBuilder<DineConnectContext>()
                .UseSqlite(conn)
                .EnableSensitiveDataLogging()
                .Options;

            var ctx = new DineConnectContext(options);
            ctx.Database.EnsureCreated();
            return (ctx, conn);
        }

        /// <summary>
        /// Creates and seeds an in-memory database context for testing.
        /// </summary>
        public static async Task<(DineConnectContext Ctx, SqliteConnection Conn)> CreateAndSeedAsync()
        {
            var (ctx, conn) = CreateInMemory();
            await DbSeed.EnsureCreatedAndSeedAsync(ctx);
            return (ctx, conn);
        }
    }

    // ---------- USERS ----------
    [TestFixture]
    public class UserTests
    {
        /// <summary>
        /// Ensures that two users are seeded with expected fields and unique usernames.
        /// </summary>
        [Test]
        public async Task Seeds_TwoUsers_WithExpectedFields_AndUniqueUserNames()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                Assert.That(ctx.Users.Count(), Is.EqualTo(2), "Should seed exactly 2 users");

                var alice = ctx.Users.SingleOrDefault(u => u.Id == 10000001);
                var bob = ctx.Users.SingleOrDefault(u => u.Id == 10000002);

                Assert.That(alice, Is.Not.Null);
                Assert.That(bob, Is.Not.Null);
                Assert.That(alice!.UserName, Is.EqualTo("alice"));
                Assert.That(bob!.UserName, Is.EqualTo("bob"));
                Assert.That(alice.PasswordHash, Is.Not.Empty);
                Assert.That(bob.PasswordHash, Is.Not.Empty);

                var distinctNames = ctx.Users.Select(u => u.UserName).Distinct().Count();
                Assert.That(distinctNames, Is.EqualTo(2));
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }

        /// <summary>
        /// Ensures that seeding users twice does not create duplicates (idempotency).
        /// </summary>
        [Test]
        public async Task Idempotent_WhenSeedingUsersTwice_DoesNotDuplicate()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                var firstCount = ctx.Users.Count();
                await DbSeed.EnsureCreatedAndSeedAsync(ctx);
                Assert.That(ctx.Users.Count(), Is.EqualTo(firstCount));
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }
    }

    // ---------- RESTAURANTS ----------
    [TestFixture]
    public class RestaurantTests
    {
        /// <summary>
        /// Ensures that two restaurants are seeded with valid geo and contact information.
        /// </summary>
        [Test]
        public async Task Seeds_TwoRestaurants_WithValidGeoAndContact()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                Assert.That(ctx.Restaurants.Count(), Is.EqualTo(2));

                var ocean = ctx.Restaurants.SingleOrDefault(r => r.Id == 20000001);
                var mountain = ctx.Restaurants.SingleOrDefault(r => r.Id == 20000002);

                Assert.That(ocean, Is.Not.Null);
                Assert.That(mountain, Is.Not.Null);

                Assert.That(ocean!.Name, Is.EqualTo("Ocean View Grill"));
                Assert.That(ocean.Address, Is.Not.Empty);
                Assert.That(ocean.Lat, Is.InRange(-90.0, 90.0));
                Assert.That(ocean.Lng, Is.InRange(-180.0, 180.0));
                Assert.That(ocean.Phone, Is.Not.Empty);

                Assert.That(mountain!.Name, Is.EqualTo("Mountain Top Diner"));
                Assert.That(mountain.Address, Is.Not.Empty);
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }

        /// <summary>
        /// Ensures that seeding restaurants twice does not create duplicates (idempotency).
        /// </summary>
        [Test]
        public async Task Idempotent_WhenSeedingRestaurantsTwice_DoesNotDuplicate()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                var first = ctx.Restaurants.Count();
                await DbSeed.EnsureCreatedAndSeedAsync(ctx);
                Assert.That(ctx.Restaurants.Count(), Is.EqualTo(first));
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }
    }

    // ---------- POSTS ----------
    [TestFixture]
    public class PostTests
    {
        /// <summary>
        /// Ensures that two posts are seeded with expected titles and valid user links.
        /// </summary>
        [Test]
        public async Task Seeds_TwoPosts_WithExpectedTitles_AndValidUserLinks()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                Assert.That(ctx.Posts.Count(), Is.EqualTo(2));

                var p1 = ctx.Posts.SingleOrDefault(p => p.Id == 30000001);
                var p2 = ctx.Posts.SingleOrDefault(p => p.Id == 30000002);

                Assert.That(p1, Is.Not.Null);
                Assert.That(p2, Is.Not.Null);

                Assert.That(p1!.Title, Does.Contain("Seafood"));
                Assert.That(p2!.Title, Does.Contain("Love this place"));

                // Validate user foreign keys by existence in Users
                Assert.That(ctx.Users.Any(u => u.Id == p1.UserId), "Post1.UserId must exist in Users");
                Assert.That(ctx.Users.Any(u => u.Id == p2.UserId), "Post2.UserId must exist in Users");

                // Content sanity
                Assert.That(p1.Content, Is.Not.Empty);
                Assert.That(p2.Content, Is.Not.Empty);
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }

        /// <summary>
        /// Ensures that querying posts by user ID returns only that user's posts.
        /// </summary>
        [Test]
        public async Task Query_ByUserId_ReturnsOnlyThatUsersPosts()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                var alicesPosts = ctx.Posts.Where(p => p.UserId == 10000001).ToList();
                Assert.That(alicesPosts.Count, Is.EqualTo(1));
                Assert.That(alicesPosts[0].Id, Is.EqualTo(30000001));
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }
    }

    // ---------- COMMENTS ----------
    [TestFixture]
    public class CommentTests
    {
        /// <summary>
        /// Ensures that two comments are seeded with valid post and user links.
        /// </summary>
        [Test]
        public async Task Seeds_TwoComments_WithValidPostAndUserLinks()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                Assert.That(ctx.Comments.Count(), Is.EqualTo(2));

                var c1 = ctx.Comments.SingleOrDefault(c => c.Id == 40000001);
                var c2 = ctx.Comments.SingleOrDefault(c => c.Id == 40000002);

                Assert.That(c1, Is.Not.Null);
                Assert.That(c2, Is.Not.Null);

                Assert.That(ctx.Posts.Any(p => p.Id == c1!.PostId));
                Assert.That(ctx.Posts.Any(p => p.Id == c2!.PostId));
                Assert.That(ctx.Users.Any(u => u.Id == c1.UserId));
                Assert.That(ctx.Users.Any(u => u.Id == c2.UserId));

                Assert.That(c1.Content, Is.Not.Empty);
                Assert.That(c2.Content, Is.Not.Empty);
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }

        /// <summary>
        /// Ensures that querying comments for a post returns the expected items.
        /// </summary>
        [Test]
        public async Task Query_CommentsForPost_ReturnsExpectedItems()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                var commentsOnPost1 = ctx.Comments.Where(c => c.PostId == 30000001).ToList();
                Assert.That(commentsOnPost1.Count, Is.EqualTo(1));
                Assert.That(commentsOnPost1[0].Id, Is.EqualTo(40000001));
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }
    }

    // ---------- RESERVATIONS ----------
    [TestFixture]
    public class ReservationTests
    {
        [Test]
        public async Task Seeds_TwoReservations_WithValidUserAndRestaurantLinks_AndReasonableDates()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                Assert.That(ctx.Reservations.Count(), Is.EqualTo(2));

                var r1 = ctx.Reservations.SingleOrDefault(r => r.Id == 50000001);
                var r2 = ctx.Reservations.SingleOrDefault(r => r.Id == 50000002);

                Assert.That(r1, Is.Not.Null);
                Assert.That(r2, Is.Not.Null);

                Assert.That(ctx.Users.Any(u => u.Id == r1!.UserId), "r1 User must exist");
                Assert.That(ctx.Restaurants.Any(s => s.Id == r1.RestaurantId), "r1 Restaurant must exist");
                Assert.That(ctx.Users.Any(u => u.Id == r2!.UserId), "r2 User must exist");
                Assert.That(ctx.Restaurants.Any(s => s.Id == r2.RestaurantId), "r2 Restaurant must exist");

                var lowerBound = DateTime.Now.AddDays(-1);
                Assert.That(r1.At, Is.GreaterThan(lowerBound));
                Assert.That(r2.At, Is.GreaterThan(lowerBound));

                Assert.That(r1.PartySize, Is.GreaterThan(0));
                Assert.That(r2.PartySize, Is.GreaterThan(0));

                Assert.That(r1.Status, Is.EqualTo(ReservationStatus.Confirmed));
                Assert.That(r2.Status, Is.EqualTo(ReservationStatus.Pending));
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }

        [Test]
        public async Task Query_ByUser_ReturnsOnlyThatUsersReservations()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                var alices = ctx.Reservations.Where(r => r.UserId == 10000001).ToList();
                Assert.That(alices.Count, Is.EqualTo(1));
                Assert.That(alices[0].RestaurantId, Is.EqualTo(20000001));
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }

        [Test]
        public async Task Idempotent_WhenSeedingReservationsTwice_DoesNotDuplicate()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                var first = ctx.Reservations.Count();
                await DbSeed.EnsureCreatedAndSeedAsync(ctx);
                Assert.That(ctx.Reservations.Count(), Is.EqualTo(first));
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }
    }

    // ---------- WHOLE-DATABASE TEST ----------
    [TestFixture]
    public class WholeDatabaseTests
    {
        [Test]
        public async Task SeedingTwice_IsFullyIdempotent_AcrossAllEntities()
        {
            var (ctx, conn) = await TestDb.CreateAndSeedAsync();
            try
            {
                var snapshot = new
                {
                    Users = ctx.Users.Count(),
                    Restaurants = ctx.Restaurants.Count(),
                    Posts = ctx.Posts.Count(),
                    Comments = ctx.Comments.Count(),
                    Reservations = ctx.Reservations.Count()
                };

                await DbSeed.EnsureCreatedAndSeedAsync(ctx);

                Assert.Multiple(() =>
                {
                    Assert.That(ctx.Users.Count(), Is.EqualTo(snapshot.Users));
                    Assert.That(ctx.Restaurants.Count(), Is.EqualTo(snapshot.Restaurants));
                    Assert.That(ctx.Posts.Count(), Is.EqualTo(snapshot.Posts));
                    Assert.That(ctx.Comments.Count(), Is.EqualTo(snapshot.Comments));
                    Assert.That(ctx.Reservations.Count(), Is.EqualTo(snapshot.Reservations));
                });
            }
            finally { ctx.Dispose(); conn.Dispose(); }
        }
    }
}
