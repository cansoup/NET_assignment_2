using DineConnect.App.Data;
using DineConnect.App.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DineConnect.App.Views
{
    public partial class CommunityView : UserControl
    {
        private readonly DineConnectContext _db;
        private readonly ObservableCollection<PostRow> _feed = new();
        private bool _isLoaded;

        public CommunityView()
        {
            InitializeComponent();
            _db = new DineConnectContext();
            Loaded += CommunityView_Loaded;
            Unloaded += CommunityView_Unloaded;
        }

        private async void CommunityView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await DbSeed.EnsureCreatedAndSeedAsync(_db);
                FeedList.ItemsSource = _feed;

                await LoadFeedAsync();

                _isLoaded = true;
                ValidatePostForm();
            }
            catch (Exception ex)
            {
                LeftStatusText.Text = $"Initialization error: {ex.Message}";
            }
        }

        private void CommunityView_Unloaded(object sender, RoutedEventArgs e)
        {
            _db?.Dispose();
        }

        private async Task LoadFeedAsync()
        {
            _feed.Clear();

            var users = await _db.Users.AsNoTracking().ToDictionaryAsync(u => u.Id, u => u.UserName);

            var posts = await _db.Posts.AsNoTracking()
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var postIds = posts.Select(p => p.Id).ToArray();
            var comments = await _db.Comments.AsNoTracking()
                .Where(c => postIds.Contains(c.PostId))
                .OrderBy(c => c.Id)
                .ToListAsync();

            foreach (var post in posts)
            {
                var username = users.TryGetValue(post.UserId, out var name) ? name : "unknown";
                var row = new PostRow(post, "@" + username);

                foreach (var c in comments.Where(x => x.PostId == post.Id))
                {
                    var commentUsername = users.TryGetValue(c.UserId, out var cname) ? cname : "unknown";
                    row.Comments.Add(new CommentRow(c, "@" + commentUsername));
                }

                _feed.Add(row);
            }

            RightStatusText.Text = _feed.Count == 0 ? "No posts yet — be the first to post!" : "";
        }

        private void ValidatePostForm()
        {
            bool ok = !string.IsNullOrWhiteSpace(PostTitleText?.Text)
                   && !string.IsNullOrWhiteSpace(PostContentText?.Text);
            PublishButton.IsEnabled = ok;
            LeftStatusText.Text = ok ? "" : "Enter a title and some content to post.";
        }

        private async void PublishButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded) return;

            var title = (PostTitleText.Text ?? "").Trim();
            var content = (PostContentText.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                ValidatePostForm();
                return;
            }

            var entity = new Post
            {
                Title = title,
                Content = content,
                UserId = 10000001 // TODO: change to logged in user (currently alice)
            };

            try
            {
                await _db.Posts.AddAsync(entity);
                await _db.SaveChangesAsync();

                await LoadFeedAsync();

                PostTitleText.Text = "";
                PostContentText.Text = "";
                ValidatePostForm();

                LeftStatusText.Text = $"✅ Posted “{entity.Title}”.";
            }
            catch (DbUpdateException ex)
            {
                LeftStatusText.Text = $"Could not publish: {ex.GetBaseException().Message}";
            }
            catch (Exception ex)
            {
                LeftStatusText.Text = $"Unexpected error: {ex.Message}";
            }
        }

        private async void AddComment_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not PostRow post) return;

            var text = (post.NewCommentText ?? "").Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                RightStatusText.Text = "Write a comment first.";
                return;
            }

            var entity = new Comment
            {
                PostId = post.Id,
                Content = text,
                UserId = 10000001 // TODO: change to logged in user (currently alice)
            };

            try
            {
                await _db.Comments.AddAsync(entity);
                await _db.SaveChangesAsync();

                await LoadFeedAsync();

                RightStatusText.Text = "💬 Comment added.";
            }
            catch (Exception ex)
            {
                RightStatusText.Text = $"Error adding comment: {ex.Message}";
            }
        }

        private void PostInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidatePostForm();
        }

        // View models
        private sealed class PostRow : INotifyPropertyChanged
        {
            public PostRow(Post p, string username)
            {
                Id = p.Id;
                Title = p.Title;
                Content = p.Content;
                UserName = username;
            }

            public int Id { get; }
            public string UserName { get; }
            public string Title { get; }
            public string Content { get; }
            public ObservableCollection<CommentRow> Comments { get; } = new();

            private string _newCommentText = "";
            public string NewCommentText
            {
                get => _newCommentText;
                set { _newCommentText = value; OnPropertyChanged(nameof(NewCommentText)); }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private sealed class CommentRow
        {
            public CommentRow(Comment c, string username)
            {
                Id = c.Id;
                PostId = c.PostId;
                Content = c.Content;
                UserName = username;
            }

            public int Id { get; }
            public int PostId { get; }
            public string Content { get; }
            public string UserName { get; }
        }
    }
}
