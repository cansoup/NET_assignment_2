using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DineConnect.App.Models;
using DineConnect.App.Data; // ✅ DbContext namespace

namespace DineConnect.App.Views
{
    public partial class ReservationsView : UserControl
    {
        public ReservationsView()
        {
            InitializeComponent();
            DataContext = new ReservationsViewModel();
        }

        // ==================== VIEWMODEL ====================
        private class ReservationsViewModel : INotifyPropertyChanged
        {
            private readonly int _currentUserId = 10000001; // ✅ Fixed user as requested

            // ===== Collections =====
            public ObservableCollection<Restaurant> Restaurants { get; } = new();
            public ICollectionView FilteredRestaurants { get; }
            public ObservableCollection<Reservation> Reservations { get; } = new();
            public ObservableCollection<Reservation> ReservationsForSelected { get; } = new();

            public ReservationsViewModel()
            {
                LoadDataFromDatabase(); // ✅ Initial EF Load

                // Setup filter
                FilteredRestaurants = CollectionViewSource.GetDefaultView(Restaurants);
                FilteredRestaurants.Filter = FilterRestaurants;

                // Build time slots (17:00 to 22:00 every 30min)
                var start = DateTime.Today.AddHours(17);
                var end = DateTime.Today.AddHours(22);
                for (var t = start; t <= end; t = t.AddMinutes(30))
                    TimeSlots.Add(t.ToString("HH:mm"));

                NewReservationTime = TimeSlots.FirstOrDefault();

                CreateReservationCommand = new RelayCommand(_ => CreateReservation(), _ => CanCreate);
            }

            private void LoadDataFromDatabase()
            {
                using var db = new DineConnectContext();

                Restaurants.Clear();
                foreach (var r in db.Restaurants.ToList())
                    Restaurants.Add(r);

                Reservations.Clear();
                foreach (var res in db.Reservations.ToList())
                    Reservations.Add(res);
            }

            private void RefreshReservationsForSelected()
            {
                ReservationsForSelected.Clear();
                if (SelectedRestaurant == null) return;

                using var db = new DineConnectContext();

                var items = db.Reservations
                    .Where(r => r.RestaurantId == SelectedRestaurant.Id)
                    .OrderByDescending(r => r.At)
                    .ToList();

                foreach (var r in items)
                    ReservationsForSelected.Add(r);
            }

            // ===== Selected Restaurant =====
            private Restaurant _selectedRestaurant;
            public Restaurant SelectedRestaurant
            {
                get => _selectedRestaurant;
                set
                {
                    if (Set(ref _selectedRestaurant, value))
                    {
                        RefreshReservationsForSelected();
                        RaisePropertyChanged(nameof(CanCreate));
                        CommandManager.InvalidateRequerySuggested();
                    }
                }
            }

            // ===== Form Fields =====
            private DateTime? _newReservationDate = DateTime.Today;
            public DateTime? NewReservationDate
            {
                get => _newReservationDate;
                set
                {
                    Set(ref _newReservationDate, value);
                    RaisePropertyChanged(nameof(CanCreate));
                    CommandManager.InvalidateRequerySuggested();
                }
            }

            public ObservableCollection<string> TimeSlots { get; } = new();
            private string _newReservationTime;
            public string NewReservationTime
            {
                get => _newReservationTime;
                set
                {
                    Set(ref _newReservationTime, value);
                    RaisePropertyChanged(nameof(CanCreate));
                    CommandManager.InvalidateRequerySuggested();
                }
            }

            public ObservableCollection<int> PartySizes { get; } = new(Enumerable.Range(1, 12));
            private int _newPartySize = 2;
            public int NewPartySize
            {
                get => _newPartySize;
                set
                {
                    Set(ref _newPartySize, value);
                    RaisePropertyChanged(nameof(CanCreate));
                    CommandManager.InvalidateRequerySuggested();
                }
            }

            public Array ReservationStatuses { get; } = Enum.GetValues(typeof(ReservationStatus));
            private ReservationStatus _newStatus = ReservationStatus.Confirmed;
            public ReservationStatus NewStatus
            {
                get => _newStatus;
                set => Set(ref _newStatus, value);
            }

            // ===== Search =====
            private string _restaurantSearch = string.Empty;
            public string RestaurantSearch
            {
                get => _restaurantSearch;
                set
                {
                    if (Set(ref _restaurantSearch, value))
                        FilteredRestaurants.Refresh();
                }
            }

            private bool FilterRestaurants(object obj)
            {
                if (obj is not Restaurant r) return false;
                if (string.IsNullOrWhiteSpace(RestaurantSearch)) return true;

                var q = RestaurantSearch.Trim();
                return (r.Name?.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                       || (r.Address?.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // ===== Reservation Create Command =====
            public ICommand CreateReservationCommand { get; }
            public bool CanCreate =>
                SelectedRestaurant != null &&
                NewReservationDate.HasValue &&
                !string.IsNullOrWhiteSpace(NewReservationTime) &&
                NewPartySize > 0;

            private string _createErrorMessage;
            public string CreateErrorMessage { get => _createErrorMessage; set => Set(ref _createErrorMessage, value); }

            private string _createSuccessMessage;
            public string CreateSuccessMessage { get => _createSuccessMessage; set => Set(ref _createSuccessMessage, value); }

            private void CreateReservation()
            {
                CreateErrorMessage = CreateSuccessMessage = string.Empty;

                if (!DateTime.TryParseExact(NewReservationTime, "HH:mm", null, DateTimeStyles.None, out var parsedTime))
                {
                    CreateErrorMessage = "Invalid time.";
                    return;
                }

                var at = NewReservationDate!.Value.Date
                    .AddHours(parsedTime.Hour)
                    .AddMinutes(parsedTime.Minute);

                var newRes = new Reservation
                {
                    RestaurantId = SelectedRestaurant.Id,
                    UserId = _currentUserId,
                    At = at,
                    PartySize = NewPartySize,
                    Status = NewStatus
                };

                using var db = new DineConnectContext();
                db.Reservations.Add(newRes);
                db.SaveChanges();

                CreateSuccessMessage = $"Reservation created for {SelectedRestaurant.Name} at {at:g}.";

                RefreshReservationsForSelected(); // ✅ Reload from DB
            }

            // ===== PropertyChanged =====
            public event PropertyChangedEventHandler PropertyChanged;
            private void RaisePropertyChanged([CallerMemberName] string prop = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

            protected bool Set<T>(ref T field, T value, [CallerMemberName] string prop = null)
            {
                if (Equals(field, value)) return false;
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
                return true;
            }
        }

        // ===== RelayCommand helper =====
        private class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
            public void Execute(object parameter) => _execute(parameter);

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }
    }
}
