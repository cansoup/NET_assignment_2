using DineConnect.App.Models;
using DineConnect.App.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DineConnect.App.Services
{
    /// <summary>
    /// Service layer for community feed (posts & comments). Encapsulates all EF/data access.
    /// </summary>
    public sealed class CommunityService : IDisposable, IInitializableService
    {
        private readonly DineConnectContext _db;
        private bool _disposed;

        public CommunityService(DineConnectContext? context = null)
        {
            _db = context ?? new DineConnectContext();
        }

        public async Task EnsureInitializedAsync()
        {
            await DbSeed.EnsureCreatedAndSeedAsync(_db);
        }

        public sealed class CommentRow
        {
            public int Id { get; init; }
            public int PostId { get; init; }
            public int UserId { get; init; }
            public string UserName { get; init; } = "";
            public string Content { get; init; } = "";

            public static CommentRow From(Comment c, string userName) => new()
            {
                Id = c.Id,
                PostId = c.PostId,
                UserId = c.UserId,
                UserName = userName,
                Content = c.Content
            };
        }

        public sealed class PostRow
        {
            public int Id { get; init; }
            public int UserId { get; init; }
            public string UserName { get; init; } = "";
            public string Title { get; init; } = "";
            public string Content { get; init; } = "";
            public List<CommentRow> Comments { get; init; } = new();
        }

        public async Task<List<PostRow>> GetFeedAsync(CancellationToken ct = default)
        {
            // Users map for usernames
            var users = await _db.Users.AsNoTracking()
                .ToDictionaryAsync(u => u.Id, u => u.UserName, ct);

            // Posts (latest first)
            var posts = await _db.Posts.AsNoTracking()
                .OrderByDescending(p => p.Id)
                .ToListAsync(ct);

            var postIds = posts.Select(p => p.Id).ToArray();

            // All comments for these posts
            var comments = await _db.Comments.AsNoTracking()
                .Where(c => postIds.Contains(c.PostId))
                .OrderBy(c => c.Id)
                .ToListAsync(ct);

            // Build rows
            var rows = new List<PostRow>(posts.Count);
            foreach (var p in posts)
            {
                var postUser = users.TryGetValue(p.UserId, out var pu) ? pu : "unknown";
                var row = new PostRow
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserName = "@" + postUser,
                    Title = p.Title,
                    Content = p.Content
                };

                foreach (var c in comments.Where(x => x.PostId == p.Id))
                {
                    var commentUser = users.TryGetValue(c.UserId, out var cu) ? cu : "unknown";
                    row.Comments.Add(CommentRow.From(c, "@" + commentUser));
                }

                rows.Add(row);
            }

            return rows;
        }

        public sealed record CreatePostResult(bool Ok, string? Error, PostRow? Created);

        public async Task<CreatePostResult> CreatePostAsync(int userId, string title, string content, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(title)) return new(false, "Title is required.", null);
            if (string.IsNullOrWhiteSpace(content)) return new(false, "Content is required.", null);

            var entity = new Post
            {
                Title = title.Trim(),
                Content = content.Trim(),
                UserId = userId
            };

            try
            {
                await _db.Posts.AddAsync(entity, ct);
                await _db.SaveChangesAsync(ct);

                var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
                var userName = user?.UserName ?? "unknown";

                var row = new PostRow
                {
                    Id = entity.Id,
                    UserId = userId,
                    UserName = "@" + userName,
                    Title = entity.Title,
                    Content = entity.Content,
                    Comments = new List<CommentRow>()
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

        public sealed record AddCommentResult(bool Ok, string? Error, CommentRow? Created);

        public async Task<AddCommentResult> AddCommentAsync(int userId, int postId, string content, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(content)) return new(false, "Comment cannot be empty.", null);

            // ensure post exists
            var postExists = await _db.Posts.AsNoTracking()
                .AnyAsync(p => p.Id == postId, ct);
            if (!postExists) return new(false, $"Post #{postId} not found.", null);

            var entity = new Comment
            {
                PostId = postId,
                Content = content.Trim(),
                UserId = userId
            };

            try
            {
                await _db.Comments.AddAsync(entity, ct);
                await _db.SaveChangesAsync(ct);

                var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
                var userName = user?.UserName ?? "unknown";

                var row = CommentRow.From(entity, "@" + userName);
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

        public void Dispose()
        {
            if (_disposed) return;
            _db.Dispose();
            _disposed = true;
        }
    }
}
