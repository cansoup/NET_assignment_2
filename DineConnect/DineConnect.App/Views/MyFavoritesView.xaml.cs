using DineConnect.App.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DineConnect.App.Views
{
    /// <summary>
    /// Interaction logic for MyFavoritesView.xaml
    /// </summary>
    public partial class MyFavoritesView : UserControl
    {
        private readonly GooglePlacesService _placeService;
        private readonly FavoriteService _favoriteService;
        private readonly DineConnectContext _dbContext;
        private DispatcherTimer _debounceTimer;

        public MyFavoritesView()
        {
            InitializeComponent();
            _placeService = new GooglePlacesService();
            _dbContext = new DineConnectContext();
            _favoriteService = new FavoriteService(_dbContext);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // debounceTimer for search input
            _debounceTimer = new DispatcherTimer();
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(500);
            _debounceTimer.Tick += OnDebounceTimerTick;

            await LoadFavoritesAsync();
        }

        // timer tick event: google API
        private async void OnDebounceTimerTick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            var searchText = SearchTextBox.Text;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ResultsListBox.Visibility = Visibility.Collapsed;
                return;
            }

            var suggestions = await _placeService.GetAutocompleteSuggestionAsync(searchText);

            if (suggestions != null && suggestions.Any())
            {
                ResultsListBox.ItemsSource = suggestions;
                ResultsListBox.Visibility = Visibility.Visible;
            }
            else
            {
                ResultsListBox.Visibility = Visibility.Collapsed;
            }
        }

        // timer restarted on text changed
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private async void AddFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is not Prediction selectedPlace)
            {
                MessageBox.Show("Please select a restaurant from the list.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RatingComboBox.SelectedItem is not ComboBoxItem selectedRatingItem)
            {
                MessageBox.Show("Please select a rating.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("Error: No user is logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var rating = int.Parse(selectedRatingItem.Content.ToString().Split(' ')[0]);

            var parts = selectedPlace.description.Split(new[] { ',' }, 2);
            var name = parts[0].Trim();
            var address = (parts.Length > 1) ? parts[1].Trim() : "";

            bool added = await _favoriteService.AddFavoriteAsync(AppState.CurrentUser.Id, name, address, rating);

            if (added)
            {
                MessageBox.Show($"{name} has been added to your favorites!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadFavoritesAsync();

                SearchTextBox.Clear();
                RatingComboBox.SelectedIndex = -1;
                ResultsListBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("This restaurant is already in your favorites.", "Already Exists", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task LoadFavoritesAsync()
        {
            if (AppState.CurrentUser == null) return;

            var favorites = await _favoriteService.GetFavoritesForUserAsync(AppState.CurrentUser.Id);
            if (favorites.Any())
            {
                FavoritesListView.ItemsSource = favorites;
                FavoritesListView.Visibility = Visibility.Visible;
                NoFavoritesText.Visibility = Visibility.Collapsed;
            }
            else
            {
                FavoritesListView.Visibility = Visibility.Collapsed;
                NoFavoritesText.Visibility = Visibility.Visible;
            }   
        }

        private void ResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is Prediction selected)
            {
                SearchTextBox.TextChanged -= SearchTextBox_TextChanged;
                SearchTextBox.Text = selected.description;
                SearchTextBox.TextChanged += SearchTextBox_TextChanged;

                ResultsListBox.Visibility = Visibility.Collapsed;
                _debounceTimer.Stop();
            }
        }
    }
}
