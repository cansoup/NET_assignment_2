using DineConnect.App.Data;
using DineConnect.App.Models;
using DineConnect.App.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DineConnect.App.Views
{
    public partial class ReservationsView : UserControl
    {
        // EF context
        private readonly DineConnectContext _db;

        // UI state
        private readonly ObservableCollection<ReservationRow> _reservations = new();
        private List<RestaurantItem> _restaurants = new();
        private bool _isLoaded;

        public ReservationsView()
        {
            InitializeComponent();
            _db = new DineConnectContext();
            Loaded += ReservationsView_Loaded;
            Unloaded += ReservationsView_Unloaded;
        }

        private async void ReservationsView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Seed & ensure DB is there
                await DbSeed.EnsureCreatedAndSeedAsync(_db);

                // 1) Bind UI collections
                ReservationsGrid.ItemsSource = _reservations;

                // 2) Initialize date/time + party
                DatePicker.DisplayDateStart = DateTime.Today;
                DatePicker.SelectedDate = DateTime.Today;
                PopulateTimeSlots(DateTime.Today);
                PartySlider.Value = 2;
                PartyText.Text = "2";

                // 3) Load EF data
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
            var list = await _db.Restaurants
                                .AsNoTracking()
                                .OrderBy(r => r.Name)
                                .Select(r => new RestaurantItem { Id = r.Id, Name = r.Name })
                                .ToListAsync();

            _restaurants = list;
            RestaurantCombo.ItemsSource = _restaurants;
        }

        private async Task LoadReservationsAsync()
        {
            _reservations.Clear();

            var userId = AppState.CurrentUser.Id;

            var reservations = await _db.Reservations
                                        .AsNoTracking()
                                        .Where(r => r.UserId == userId) // only current user
                                        .OrderBy(r => r.At)
                                        .ToListAsync();

            var restNames = _restaurants.ToDictionary(r => r.Id, r => r.Name);

            foreach (var r in reservations)
            {
                var name = restNames.TryGetValue(r.RestaurantId, out var n) ? n : $"#{r.RestaurantId}";
                _reservations.Add(new ReservationRow(r, name));
            }
        }

        private void ReservationsView_Unloaded(object sender, RoutedEventArgs e)
        {
            _db?.Dispose();
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

            bool hasRestaurant = RestaurantCombo?.SelectedItem is RestaurantItem;
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

            if (RestaurantCombo.SelectedItem is not RestaurantItem restaurant ||
                TimeCombo.SelectedItem is not DateTime at)
            {
                ValidateForm();
                return;
            }

            int party = int.TryParse(PartyText.Text, out var p) ? p : (int)PartySlider.Value;

            var entity = new Reservation
            {
                RestaurantId = restaurant.Id,
                UserId = AppState.CurrentUser.Id,
                At = at,
                PartySize = party,
                Status = ReservationStatus.Confirmed
            };

            try
            {
                await _db.Reservations.AddAsync(entity);
                await _db.SaveChangesAsync();

                var name = restaurant.Name;
                _reservations.Add(new ReservationRow(entity, name));

                // Reset inputs
                DatePicker.SelectedDate = DateTime.Today;
                PopulateTimeSlots(DateTime.Today);
                PartySlider.Value = 2;
                PartyText.Text = "2";

                StatusText.Text = $"✅ Reserved {name} for {party} on {entity.At:ddd, MMM d h:mm tt}.";
            }
            catch (DbUpdateException ex)
            {
                StatusText.Text = $"Could not save reservation: {ex.GetBaseException().Message}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Unexpected error: {ex.Message}";
            }
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

                    var filtered = await _db.Reservations
                                            .AsNoTracking()
                                            .Where(r => r.UserId == userId && r.Status == status) // user + status
                                            .OrderBy(r => r.At)
                                            .ToListAsync();

                    var restNames = _restaurants.ToDictionary(r => r.Id, r => r.Name);
                    foreach (var r in filtered)
                    {
                        var name = restNames.TryGetValue(r.RestaurantId, out var n) ? n : $"#{r.RestaurantId}";
                        _reservations.Add(new ReservationRow(r, name));
                    }
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
            if ((sender as Button)?.DataContext is not ReservationRow row)
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

            try
            {
                // Load tracked entity (rows were loaded AsNoTracking)
                var entity = await _db.Reservations.FindAsync(row.Id);
                if (entity is null)
                {
                    StatusText.Text = $"Reservation #{row.Id} no longer exists.";
                    // Remove from UI to keep things consistent
                    _reservations.Remove(row);
                    return;
                }

                 if (entity.UserId != AppState.CurrentUser.Id)
                {
                    StatusText.Text = "You can only delete your own reservations.";
                    return;
                }

                _db.Reservations.Remove(entity);
                await _db.SaveChangesAsync();

                _reservations.Remove(row);

                StatusText.Text = $"🗑️ Deleted reservation #{row.Id} for \"{row.RestaurantName}\".";
            }
            catch (DbUpdateException ex)
            {
                StatusText.Text = $"Could not delete reservation: {ex.GetBaseException().Message}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Unexpected error while deleting: {ex.Message}";
            }
        }

        // Helper models for binding
        private sealed class RestaurantItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public override string ToString() => Name;
        }

        private sealed class ReservationRow
        {
            public ReservationRow(Reservation r, string restaurantName)
            {
                Id = r.Id;
                RestaurantId = r.RestaurantId;
                RestaurantName = restaurantName;
                UserId = r.UserId;
                At = r.At;
                PartySize = r.PartySize;
                Status = r.Status;
            }

            public int Id { get; }
            public int RestaurantId { get; }
            public string RestaurantName { get; }
            public int UserId { get; }
            public DateTime At { get; }
            public int PartySize { get; }
            public ReservationStatus Status { get; }
        }
    }
}
