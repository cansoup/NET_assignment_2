using DineConnect.App.Models;
using DineConnect.App.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DineConnect.App.Views
{
    public partial class ReservationsView : UserControl
    {
        // Service (owns its own DbContext)
        private readonly ReservationsService _service;

        // UI state
        private readonly ObservableCollection<ReservationsService.ReservationRow> _reservations = new();
        private List<ReservationsService.RestaurantItem> _restaurants = new();
        private bool _isLoaded;

        public ReservationsView()
        {
            InitializeComponent();
            _service = new ReservationsService();
            Loaded += ReservationsView_Loaded;
            Unloaded += ReservationsView_Unloaded;
        }

        private async void ReservationsView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 0) Ensure DB exists/seeded via service
                await _service.EnsureInitializedAsync();

                // 1) Bind UI collections
                ReservationsGrid.ItemsSource = _reservations;

                // 2) Initialize date/time + party
                DatePicker.DisplayDateStart = DateTime.Today;
                DatePicker.SelectedDate = DateTime.Today;
                PopulateTimeSlots(DateTime.Today);
                PartySlider.Value = 2;
                PartyText.Text = "2";

                // 3) Load data via service
                await LoadRestaurantsAsync();
                await LoadReservationsAsync();

                // 4) Wire handlers AFTER data is in place
                DatePicker.SelectedDateChanged += (_, __) =>
                {
                    if (!_isLoaded) return;
                    PopulateTimeSlots(DatePicker.SelectedDate ?? DateTime.Today);
                    ValidateForm();
                };
                PartySlider.ValueChanged += (_, __) => { if (_isLoaded) ValidateForm(); };
                PartyText.TextChanged += (_, __) => { if (_isLoaded) ValidateForm(); };
                RestaurantCombo.SelectionChanged += (_, __) => { if (_isLoaded) ValidateForm(); };
                TimeCombo.SelectionChanged += (_, __) => { if (_isLoaded) ValidateForm(); };

                // 5) Default filter
                StatusFilterCombo.SelectedIndex = 0;

                _isLoaded = true;
                ValidateForm();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Initialization error: {ex.Message}";
            }
        }

        private async Task LoadRestaurantsAsync()
        {
            _restaurants = await _service.GetRestaurantsAsync();
            RestaurantCombo.ItemsSource = _restaurants;
        }

        private async Task LoadReservationsAsync()
        {
            _reservations.Clear();

            var userId = AppState.CurrentUser.Id;
            var rows = await _service.GetReservationsForUserAsync(userId);

            foreach (var row in rows)
                _reservations.Add(row);
        }

        private void ReservationsView_Unloaded(object sender, RoutedEventArgs e)
        {
            _service?.Dispose();
        }

        private void PopulateTimeSlots(DateTime date)
        {
            var open = new TimeSpan(11, 0, 0);  // 11 AM
            var close = new TimeSpan(22, 0, 0); // 10 PM
            var step = TimeSpan.FromMinutes(30);

            var times = new List<DateTime>();
            for (var t = open; t <= close; t += step)
            {
                var at = date.Date.Add(t);
                if (date.Date == DateTime.Today && at <= DateTime.Now) continue;
                times.Add(at);
            }

            // Items are DateTime; XAML ItemStringFormat shows "h:mm tt"
            TimeCombo.ItemsSource = times;
            TimeCombo.SelectedIndex = times.Count > 0 ? 0 : -1;
        }

        private void ValidateForm()
        {
            if (!_isLoaded) { BookButton.IsEnabled = false; return; }

            bool hasRestaurant = RestaurantCombo?.SelectedItem is ReservationsService.RestaurantItem;
            bool hasDate = DatePicker?.SelectedDate is DateTime;
            bool hasTime = TimeCombo?.SelectedItem is DateTime;
            int maxParty = (int)PartySlider.Maximum;
            bool partyOk = int.TryParse(PartyText?.Text, out int p) && p >= 1 && p <= maxParty;

            BookButton.IsEnabled = hasRestaurant && hasDate && hasTime && partyOk;

            StatusText.Text =
                !hasRestaurant ? "Select a restaurant to continue." :
                !hasDate ? "Choose a date for your reservation." :
                !hasTime ? "Pick an available time." :
                !partyOk ? $"Enter a party size between 1 and {maxParty}." :
                "";
        }

        private async void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded || !BookButton.IsEnabled) return;

            if (RestaurantCombo.SelectedItem is not ReservationsService.RestaurantItem restaurant ||
                TimeCombo.SelectedItem is not DateTime at)
            {
                ValidateForm();
                return;
            }

            int party = int.TryParse(PartyText.Text, out var p) ? p : (int)PartySlider.Value;

            var result = await _service.CreateReservationAsync(
                AppState.CurrentUser.Id, restaurant.Id, at, party);

            if (!result.Ok)
            {
                StatusText.Text = $"Could not save reservation: {result.Error}";
                return;
            }

            // Add to UI
            if (result.Created is not null)
                _reservations.Add(result.Created);

            // Reset inputs
            DatePicker.SelectedDate = DateTime.Today;
            PopulateTimeSlots(DateTime.Today);
            PartySlider.Value = 2;
            PartyText.Text = "2";

            StatusText.Text = $"✅ Reserved {restaurant.Name} for {party} on {at:ddd, MMM d h:mm tt}.";
        }

        private async void StatusFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;

            var selectedText =
                (StatusFilterCombo.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "All";

            try
            {
                if (selectedText.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    await LoadReservationsAsync();
                    return;
                }

                if (Enum.TryParse<ReservationStatus>(selectedText, true, out var status))
                {
                    _reservations.Clear();

                    var userId = AppState.CurrentUser.Id;
                    var rows = await _service.GetReservationsForUserByStatusAsync(userId, status);

                    foreach (var r in rows)
                        _reservations.Add(r);
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Filter error: {ex.Message}";
            }
        }

        // Delete button handler
        private async void DeleteReservation_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not ReservationsService.ReservationRow row)
            {
                StatusText.Text = "Could not determine which reservation to delete.";
                return;
            }

            // Defensive: ensure user can only delete their own reservations
            if (row.UserId != AppState.CurrentUser.Id)
            {
                StatusText.Text = "You can only delete your own reservations.";
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete reservation #{row.Id} at \"{row.RestaurantName}\" on {row.At:ddd, MMM d h:mm tt}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            var result = await _service.DeleteReservationAsync(AppState.CurrentUser.Id, row.Id);
            if (!result.Ok)
            {
                StatusText.Text = $"Could not delete reservation: {result.Error}";
                return;
            }

            _reservations.Remove(row);
            StatusText.Text = $"🗑️ Deleted reservation #{row.Id} for \"{row.RestaurantName}\".";
        }
    }
}
