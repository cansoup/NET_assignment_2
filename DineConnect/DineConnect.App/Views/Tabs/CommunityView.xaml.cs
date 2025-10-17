using DineConnect.App.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DineConnect.App.Views
{
    public partial class CommunityView : UserControl
    {
        // Service (owns its own DbContext)
        private readonly CommunityService _service;

        // UI state (keeps view-only bits like NewCommentText)
        private readonly ObservableCollection<PostItem> _feed = new();
        private bool _isLoaded;

        public CommunityView()
        {
            InitializeComponent();
            _service = new CommunityService();
            Loaded += CommunityView_Loaded;
            Unloaded += CommunityView_Unloaded;
        }

        private async void CommunityView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 0) Ensure DB exists/seeded via service
                await _service.EnsureInitializedAsync();

                // 1) Bind UI collections
                FeedList.ItemsSource = _feed;

                // 2) Load data via service
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
            _service?.Dispose();
        }

        private async Task LoadFeedAsync()
        {
            _feed.Clear();

            var rows = await _service.GetFeedAsync();

            foreach (var r in rows)
            {
                // Map service rows to lightweight view items (adds NewCommentText + observable comments)
                var item = new PostItem(r);
                _feed.Add(item);
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

            var result = await _service.CreatePostAsync(AppState.CurrentUser.Id, title, content);
            if (!result.Ok)
            {
                LeftStatusText.Text = $"Could not publish: {result.Error}";
                return;
            }

            // Reload feed (or insert at top if you prefer)
            await LoadFeedAsync();

            PostTitleText.Text = "";
            PostContentText.Text = "";
            ValidatePostForm();

            LeftStatusText.Text = $"✅ Posted “{title}”.";
        }

        private async void AddComment_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not PostItem post)
            {
                RightStatusText.Text = "Could not determine which post to comment on.";
                return;
            }

            var text = (post.NewCommentText ?? "").Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                RightStatusText.Text = "Write a comment first.";
                return;
            }

            var result = await _service.AddCommentAsync(AppState.CurrentUser.Id, post.Id, text);
            if (!result.Ok)
            {
                RightStatusText.Text = $"Error adding comment: {result.Error}";
                return;
            }

            // Reload (or append to the one PostItem.Comments)
            await LoadFeedAsync();

            RightStatusText.Text = "💬 Comment added.";
        }

        private void PostInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidatePostForm();
        }

        // View-only wrappers around the service DTOs (keeps UI-specific state out of the service)
        private sealed class PostItem : INotifyPropertyChanged
        {
            public PostItem(CommunityService.PostRow row)
            {
                Id = row.Id;
                UserId = row.UserId;
                UserName = row.UserName;
                Title = row.Title;
                Content = row.Content;
                foreach (var c in row.Comments)
                    Comments.Add(c); // already lightweight rows
            }

            public int Id { get; }
            public int UserId { get; }
            public string UserName { get; }
            public string Title { get; }
            public string Content { get; }

            // Comments are the service CommentRow(s) (no UI state per comment)
            public ObservableCollection<CommunityService.CommentRow> Comments { get; } = new();

            // UI-only state for the "add comment" textbox
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
    }
}
