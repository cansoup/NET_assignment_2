using DineConnect.App.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DineConnect.App.Views
{
    public partial class MyFavoritesView : UserControl
    {
        private readonly FavoriteService _favoriteService;
        private DispatcherTimer _debounceTimer;
        private bool _isLoaded;

        public MyFavoritesView()
        {
            InitializeComponent();
            _favoriteService = new FavoriteService(); // service owns its DbContext
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_Unloaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _favoriteService.EnsureInitializedAsync();

                _debounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                _debounceTimer.Tick += OnDebounceTimerTick;

                await LoadFavoritesAsync();

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _debounceTimer?.Stop();
            _favoriteService?.Dispose();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private async void OnDebounceTimerTick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            var searchText = SearchTextBox.Text;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ResultsListBox.ItemsSource = null;
                ResultsListBox.Visibility = Visibility.Collapsed;
                return;
            }

            try
            {
                var suggestions = await _favoriteService.SearchRestaurantsAsync(searchText, take: 20);

                if (suggestions.Any())
                {
                    // XAML expects Name/Address; RestaurantSuggestion matches that.
                    ResultsListBox.ItemsSource = suggestions;
                    ResultsListBox.Visibility = Visibility.Visible;
                }
                else
                {
                    ResultsListBox.ItemsSource = null;
                    ResultsListBox.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                ResultsListBox.ItemsSource = null;
                ResultsListBox.Visibility = Visibility.Collapsed;
            }
        }

        private async void AddFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("Error: No user is logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (RatingComboBox.SelectedItem is not ComboBoxItem selectedRatingItem)
            {
                MessageBox.Show("Please select a rating.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ratingText = selectedRatingItem.Content?.ToString() ?? "0";
            if (!int.TryParse(ratingText.Split(' ')[0], out var rating))
            {
                MessageBox.Show("Invalid rating selection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // If user selected from suggestions, use that; otherwise parse "Name, Address" or just Name
            string name;
            string address;

            if (ResultsListBox.SelectedItem is FavoriteService.RestaurantSuggestion selected)
            {
                name = selected.Name?.Trim() ?? "";
                address = selected.Address?.Trim() ?? "";
            }
            else
            {
                string input = (SearchTextBox.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Please enter or select a restaurant.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var parts = input.Split(new[] { ',' }, 2);
                name = parts[0].Trim();
                address = (parts.Length > 1) ? parts[1].Trim() : "";
            }

            var result = await _favoriteService.AddFavoriteAsync(AppState.CurrentUser.Id, name, address, rating);

            if (!result.Ok)
            {
                MessageBox.Show(result.Error ?? "Could not add favorite.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show($"{name} has been added to your favorites!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Refresh list to reflect the new item (service returns DTO with nested Restaurant)
            await LoadFavoritesAsync();

            // Reset inputs
            SearchTextBox.Clear();
            RatingComboBox.SelectedIndex = -1;
            ResultsListBox.Visibility = Visibility.Collapsed;
            ResultsListBox.ItemsSource = null;
        }

        private async Task LoadFavoritesAsync()
        {
            if (AppState.CurrentUser == null)
            {
                FavoritesListView.ItemsSource = null;
                FavoritesListView.Visibility = Visibility.Collapsed;
                NoFavoritesText.Visibility = Visibility.Visible;
                return;
            }

            var rows = await _favoriteService.GetFavoritesForUserAsync(AppState.CurrentUser.Id);

            if (rows.Any())
            {
                FavoritesListView.ItemsSource = rows; // List<FavoriteService.FavoriteRow>
                FavoritesListView.Visibility = Visibility.Visible;
                NoFavoritesText.Visibility = Visibility.Collapsed;
            }
            else
            {
                FavoritesListView.ItemsSource = null;
                FavoritesListView.Visibility = Visibility.Collapsed;
                NoFavoritesText.Visibility = Visibility.Visible;
            }
        }

        private void ResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is FavoriteService.RestaurantSuggestion selected)
            {
                SearchTextBox.TextChanged -= SearchTextBox_TextChanged;

                var display = string.IsNullOrWhiteSpace(selected.Address)
                    ? selected.Name
                    : $"{selected.Name}, {selected.Address}";

                SearchTextBox.Text = display;

                SearchTextBox.TextChanged += SearchTextBox_TextChanged;

                ResultsListBox.Visibility = Visibility.Collapsed;
                _debounceTimer.Stop();
            }
        }
    }
}
