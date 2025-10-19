using DineConnect.App.Services;
using DineConnect.App.Services.Validation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DineConnect.App.Views
{
    /// <summary>
    /// Manages the user's favorite restaurants, including searching, adding, and deleting favorites.
    /// </summary>
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

                // Wire selection changed for rating here (keeps XAML unchanged)
                RatingComboBox.SelectionChanged += RatingComboBox_SelectionChanged;

                await LoadFavoritesAsync();

                _isLoaded = true;

                // initial UI state
                ResultsListBox.Visibility = Visibility.Collapsed;
                AddFavoriteButton.IsEnabled = false;
                ValidateFavoriteForm();
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

            // cleanup event handlers
            if (_debounceTimer != null)
                _debounceTimer.Tick -= OnDebounceTimerTick;

            RatingComboBox.SelectionChanged -= RatingComboBox_SelectionChanged;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;

            _debounceTimer.Stop();
            _debounceTimer.Start();

            // Re-evaluate form validity as user types
            ValidateFavoriteForm();
        }

        private void RatingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;
            ValidateFavoriteForm();
        }

        private void ResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is FavoriteService.RestaurantSuggestion selected)
            {
                // Prevent recursive TextChanged handling while we update the textbox
                SearchTextBox.TextChanged -= SearchTextBox_TextChanged;

                var display = string.IsNullOrWhiteSpace(selected.Address)
                    ? selected.Name
                    : $"{selected.Name}, {selected.Address}";

                SearchTextBox.Text = display;

                SearchTextBox.TextChanged += SearchTextBox_TextChanged;

                ResultsListBox.Visibility = Visibility.Collapsed;
                _debounceTimer.Stop();
            }

            ValidateFavoriteForm();
        }

        private async void OnDebounceTimerTick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            var searchText = (SearchTextBox.Text ?? string.Empty).Trim();

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

            // Resolve rating from combo
            if (!TryGetSelectedRating(out var rating))
            {
                // Shouldn’t happen because button disable logic guards this,
                // but keep as defensive fallback
                MessageBox.Show("Please select a rating.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Resolve name/address from selection or manual input
            var (name, address) = ResolveNameAndAddress();

            // Defensive re-validation (button enable ensures validity already)
            var restaurantValidation = ValidateRestaurant.ValidateUpsert(name, address);
            var ratingValidation = ValidateRating.Validate(rating);
            if (!restaurantValidation.IsValid || !ratingValidation.IsValid)
            {
                var errors = restaurantValidation.Errors.Concat(ratingValidation.Errors);
                MessageBox.Show(string.Join("\n", errors), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = await _favoriteService.AddFavoriteAsync(AppState.CurrentUser.Id, name, address, rating);

            if (!result.Ok)
            {
                MessageBox.Show(result.Error ?? "Could not add favorite.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show($"{name} has been added to your favorites!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadFavoritesAsync();

            // Reset inputs
            SearchTextBox.Clear();
            RatingComboBox.SelectedIndex = -1;
            ResultsListBox.Visibility = Visibility.Collapsed;
            ResultsListBox.ItemsSource = null;

            ValidateFavoriteForm();
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

        private void ValidateFavoriteForm()
        {
            // Resolve rating
            var ratingValid = TryGetSelectedRating(out var rating) && ValidateRating.Validate(rating).IsValid;

            // Resolve current name/address
            var (name, address) = ResolveNameAndAddress();
            var restaurantValid = ValidateRestaurant.ValidateUpsert(name, address).IsValid;

            AddFavoriteButton.IsEnabled = restaurantValid && ratingValid;
        }

        private bool TryGetSelectedRating(out int rating)
        {
            rating = -1;
            if (RatingComboBox.SelectedItem is not ComboBoxItem selectedRatingItem)
                return false;

            var ratingText = selectedRatingItem.Content?.ToString() ?? string.Empty;
            var token = ratingText.Split(' ').FirstOrDefault();
            return int.TryParse(token, out rating);
        }

        private (string name, string address) ResolveNameAndAddress()
        {
            if (ResultsListBox.SelectedItem is FavoriteService.RestaurantSuggestion selected)
            {
                var selName = (selected.Name ?? string.Empty).Trim();
                var selAddr = (selected.Address ?? string.Empty).Trim();
                return (selName, selAddr);
            }

            var input = (SearchTextBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(input))
                return (string.Empty, string.Empty);

            var parts = input.Split(new[] { ',' }, 2);
            var name = parts[0].Trim();
            var address = (parts.Length > 1) ? parts[1].Trim() : string.Empty;
            return (name, address);
        }

        private async void DeleteFavorite_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not FavoriteService.FavoriteRow row)
            {
                MessageBox.Show("Could not determine which favorite to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("Error: No user is logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var name = row.Restaurant?.Name ?? "(Unknown)";
            var confirm = MessageBox.Show(
                $"Delete favorite \"{name}\"?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            var result = await _favoriteService.DeleteFavoriteAsync(AppState.CurrentUser.Id, row.FavoriteId);

            if (!result.Ok)
            {
                MessageBox.Show(result.Error ?? "Could not delete favorite.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await LoadFavoritesAsync();
            MessageBox.Show($"🗑️ Deleted favorite \"{name}\".", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

            // Refresh form state
            ValidateFavoriteForm();
        }
    }
}
