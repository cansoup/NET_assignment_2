using DineConnect.App.Services;
using DineConnect.App.Services.Validation; // ⬅️ import validators
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DineConnect.App.Views
{
    /// <summary>
    /// Displays the community feed, allowing users to view posts, publish new posts, and add comments.
    /// </summary>
    public partial class CommunityView : UserControl
    {
        private readonly CommunityService _service;
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
                await _service.EnsureInitializedAsync();
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
            _service?.Dispose();
        }

        private async Task LoadFeedAsync()
        {
            _feed.Clear();
            var rows = await _service.GetFeedAsync();

            foreach (var r in rows)
                _feed.Add(new PostItem(r));

            RightStatusText.Text = _feed.Count == 0 ? "No posts yet — be the first to post!" : "";
        }

        private void ValidatePostForm()
        {
            var title = (PostTitleText?.Text ?? string.Empty).Trim();
            var content = (PostContentText?.Text ?? string.Empty).Trim();

            var v = ValidatePost.ValidateCreateInput(title, content);

            PublishButton.IsEnabled = v.IsValid;
            LeftStatusText.Text = v.IsValid ? "" : string.Join("\n", v.Errors);
        }

        private async void PublishButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded) return;

            var title = (PostTitleText.Text ?? string.Empty).Trim();
            var content = (PostContentText.Text ?? string.Empty).Trim();

            var v = ValidatePost.ValidateCreateInput(title, content);
            if (!v.IsValid)
            {
                PublishButton.IsEnabled = false;
                LeftStatusText.Text = string.Join("\n", v.Errors);
                return;
            }

            var result = await _service.CreatePostAsync(AppState.CurrentUser.Id, title, content);
            if (!result.Ok)
            {
                LeftStatusText.Text = $"Could not publish: {result.Error}";
                return;
            }

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

            var text = (post.NewCommentText ?? string.Empty).Trim();

            var v = ValidateComment.ValidateCreateInput(text);
            if (!v.IsValid)
            {
                RightStatusText.Text = string.Join("\n", v.Errors);
                return;
            }

            var result = await _service.AddCommentAsync(AppState.CurrentUser.Id, post.Id, text);
            if (!result.Ok)
            {
                RightStatusText.Text = $"Error adding comment: {result.Error}";
                return;
            }

            await LoadFeedAsync();

            RightStatusText.Text = "💬 Comment added.";
        }

        private void PostInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidatePostForm();
        }

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
                    Comments.Add(c);
            }

            public int Id { get; }
            public int UserId { get; }
            public string UserName { get; }
            public string Title { get; }
            public string Content { get; }
            public ObservableCollection<CommunityService.CommentRow> Comments { get; } = new();

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
