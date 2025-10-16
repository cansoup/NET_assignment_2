using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DineConnect.App.Models;

namespace DineConnect.App.Views
{
    public partial class ReservationsView : UserControl
    {
        private readonly ObservableCollection<ReservationRow> _reservations = new();
        private readonly List<RestaurantItem> _restaurants = new()
        {
            new RestaurantItem { Id = 1, Name = "Saffron Garden" },
            new RestaurantItem { Id = 2, Name = "Harbour & Vine" },
            new RestaurantItem { Id = 3, Name = "Trattoria Lucca" },
            new RestaurantItem { Id = 4, Name = "Katsu & Co." },
        };

        private int _nextReservationId = 1;
        private readonly int _currentUserId = 42;
        private bool _isLoaded; // prevents early event firing from touching nulls

        public ReservationsView()
        {
            InitializeComponent();

            // Wire minimal, safe listeners (anything that might query other controls waits until Loaded)
            Loaded += ReservationsView_Loaded;
        }

        private void ReservationsView_Loaded(object? sender, RoutedEventArgs e)
        {
            try
            {
                // 1) Bind data sources first
                RestaurantCombo.ItemsSource = _restaurants;
                ReservationsGrid.ItemsSource = _reservations;

                // 2) Initialize date/time
                DatePicker.DisplayDateStart = DateTime.Today;
                DatePicker.SelectedDate = DateTime.Today;
                PopulateTimeSlots(DateTime.Today);

                // 3) Initialize party size
                PartySlider.Value = 2;
                PartyText.Text = "2";

                // 4) Now it’s safe to attach reactive handlers
                DatePicker.SelectedDateChanged += (_, __) =>
                {
                    if (!_isLoaded) return;
                    var date = DatePicker.SelectedDate ?? DateTime.Today;
                    PopulateTimeSlots(date);
                    ValidateForm();
                };
                PartySlider.ValueChanged += (_, __) => { if (_isLoaded) ValidateForm(); };
                PartyText.TextChanged += (_, __) => { if (_isLoaded) ValidateForm(); };
                RestaurantCombo.SelectionChanged += (_, __) => { if (_isLoaded) ValidateForm(); };
                TimeCombo.SelectionChanged += (_, __) => { if (_isLoaded) ValidateForm(); };

                // 5) Status filter default (do this AFTER ItemsSource is set to avoid nulls)
                StatusFilterCombo.SelectedIndex = 0;

                _isLoaded = true;
                ValidateForm();
            }
            catch (Exception ex)
            {
                // Helpful diagnostic in case something else bubbles up
                StatusText.Text = $"Initialization error: {ex.Message}";
            }
        }

        private void PopulateTimeSlots(DateTime date)
        {
            var open = new TimeSpan(11, 0, 0);
            var close = new TimeSpan(22, 0, 0);
            var step = TimeSpan.FromMinutes(30);

            var items = new List<TimeSlot>();
            for (var t = open; t <= close; t += step)
            {
                var at = date.Date + t;
                if (date.Date == DateTime.Today && at <= DateTime.Now) continue;
                items.Add(new TimeSlot { At = at, Label = at.ToString("h:mm tt") });
            }

            TimeCombo.ItemsSource = items;
            TimeCombo.DisplayMemberPath = nameof(TimeSlot.Label);
            TimeCombo.SelectedValuePath = nameof(TimeSlot.At);
            TimeCombo.SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        private void ValidateForm()
        {
            if (!_isLoaded) { BookButton.IsEnabled = false; return; }

            bool hasRestaurant = RestaurantCombo?.SelectedItem is RestaurantItem;
            bool hasDate = DatePicker?.SelectedDate is DateTime;
            bool hasTime = TimeCombo?.SelectedItem is TimeSlot;
            bool partyOk = int.TryParse(PartyText?.Text, out int p) && p >= 1 && p <= 20;

            BookButton.IsEnabled = hasRestaurant && hasDate && hasTime && partyOk;

            StatusText.Text =
                !hasRestaurant ? "Select a restaurant to continue." :
                !hasDate ? "Choose a date for your reservation." :
                !hasTime ? "Pick an available time." :
                !partyOk ? "Enter a party size between 1 and 20." :
                "";
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded || !BookButton.IsEnabled) return;

            if (RestaurantCombo.SelectedItem is not RestaurantItem restaurant ||
                TimeCombo.SelectedItem is not TimeSlot timeSlot)
            {
                ValidateForm();
                return;
            }

            int party = int.TryParse(PartyText.Text, out var p) ? p : (int)PartySlider.Value;

            var reservation = new Reservation
            {
                Id = _nextReservationId++,
                RestaurantId = restaurant.Id,
                UserId = _currentUserId,
                At = timeSlot.At,
                PartySize = party,
                Status = ReservationStatus.Confirmed
            };

            _reservations.Add(new ReservationRow(reservation, restaurant.Name));

            // Reset some fields for convenience
            DatePicker.SelectedDate = DateTime.Today;
            PopulateTimeSlots(DateTime.Today);
            PartySlider.Value = 2;
            PartyText.Text = "2";

            StatusText.Text = $"✅ Reserved {restaurant.Name} for {party} on {reservation.At:ddd, MMM d h:mm tt}.";
        }

        private void StatusFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;

            var selectedText =
                (StatusFilterCombo.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "All";

            if (selectedText.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                ReservationsGrid.ItemsSource = _reservations;
                return;
            }

            if (Enum.TryParse<ReservationStatus>(selectedText, ignoreCase: true, out var status))
            {
                // Project a filtered view; keep the original backing collection intact
                ReservationsGrid.ItemsSource = new ObservableCollection<ReservationRow>(
                    _reservations.Where(r => r.Status == status));
            }
        }

        // Helper types
        private sealed class RestaurantItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }

        private sealed class TimeSlot
        {
            public DateTime At { get; set; }
            public string Label { get; set; } = "";
            public override string ToString() => Label;
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
