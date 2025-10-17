using DineConnect.App.Services;
using DineConnect.App.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly FavoriteService _favoriteService;
        private readonly DineConnectContext _dbContext;
        private DispatcherTimer _debounceTimer;

        public MyFavoritesView()
        {
            InitializeComponent();
            _dbContext = new DineConnectContext();
            _favoriteService = new FavoriteService(_dbContext);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // debounceTimer for search input
            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _debounceTimer.Tick += OnDebounceTimerTick;

            await LoadFavoritesAsync();
        }

        // timer tick event: query EF for restaurants
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
                // Basic contains search on Name OR Address; tweak to your needs (e.g., EF.Functions.Like for case-insensitive SQL)
                var suggestions = await _dbContext.Restaurants
                    .AsNoTracking()
                    .Where(r =>
                        r.Name.Contains(searchText) ||
                        (r.Address != null && r.Address.Contains(searchText)))
                    .OrderBy(r => r.Name)
                    .Take(20)
                    .ToListAsync();

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
            catch (Exception ex)
            {
                // Optional: log ex
                ResultsListBox.ItemsSource = null;
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
            // Selected restaurant from EF suggestions is optional; user might type a new one
            Restaurant? selectedRestaurant = ResultsListBox.SelectedItem as Restaurant;

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

            var rating = int.Parse(selectedRatingItem.Content.ToString()!.Split(' ')[0]);

            // If user selected from suggestions, use that; otherwise parse the input "Name, Address" or just Name
            string input = SearchTextBox.Text?.Trim() ?? string.Empty;

            string name;
            string address;

            if (selectedRestaurant != null)
            {
                name = selectedRestaurant.Name?.Trim() ?? string.Empty;
                address = selectedRestaurant.Address?.Trim() ?? string.Empty;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Please enter or select a restaurant.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var parts = input.Split(new[] { ',' }, 2);
                name = parts[0].Trim();
                address = (parts.Length > 1) ? parts[1].Trim() : string.Empty;
            }

            bool added = await _favoriteService.AddFavoriteAsync(AppState.CurrentUser.Id, name, address, rating);

            if (added)
            {
                MessageBox.Show($"{name} has been added to your favorites!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadFavoritesAsync();

                SearchTextBox.Clear();
                RatingComboBox.SelectedIndex = -1;
                ResultsListBox.Visibility = Visibility.Collapsed;
                ResultsListBox.ItemsSource = null;
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
                FavoritesListView.ItemsSource = null;
                FavoritesListView.Visibility = Visibility.Collapsed;
                NoFavoritesText.Visibility = Visibility.Visible;
            }
        }

        private void ResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is Restaurant selected)
            {
                SearchTextBox.TextChanged -= SearchTextBox_TextChanged;
                // Mirror the previous UX: put "Name, Address" into the textbox
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
