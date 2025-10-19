using DineConnect.App.Services;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET.WindowsForms;

namespace DineConnect.App.Views.Tabs
{
    /// <summary>
    /// MapView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapView : UserControl
    {
        private readonly FavoriteService _favoriteService;

        public MapView()
        {
            InitializeComponent();
            _favoriteService = new FavoriteService();
            Loaded += MapView_Loaded;
        }

        private async void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            MapBrowser.MapProvider = GMapProviders.GoogleMap;

            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            MapBrowser.DragButton = MouseButton.Left;

            MapBrowser.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            MapBrowser.MinZoom = 2;
            MapBrowser.MaxZoom = 19;
            MapBrowser.Zoom = 15;

            MapBrowser.Position = new PointLatLng(-33.8834, 151.2069);

            MapBrowser.ShowCenter = false;

            await LoadFavoriteMarkersAsync();
        }

        private async Task LoadFavoriteMarkersAsync()
        {
            if(AppState.CurrentUser == null)
            {
                return;
            }

            MapBrowser.Markers.Clear();

            try
            {
                var favoriteRows = await _favoriteService.GetFavoritesForUserAsync(AppState.CurrentUser.Id);

                if(!favoriteRows.Any())
                {
                    return;
                }

                foreach(var row in favoriteRows)
                {
                    var marker = new GMap.NET.WindowsPresentation.GMapMarker(new PointLatLng(row.Restaurant.Lat, row.Restaurant.Lng));
                    string ratingStars = new string('★', row.Rating) + new string('☆', 5 - row.Rating);

                    var shape = new Ellipse
                    {
                        Width = 12,
                        Height = 12,
                        Stroke = Brushes.White,
                        StrokeThickness = 2,
                        Fill = Brushes.Red,
                        Tag = $"{row.Restaurant.Name}\n{row.Restaurant.Address}\n{ratingStars}"
                    };

                    shape.MouseEnter += Marker_MouseEnter;
                    shape.MouseLeave += Marker_MouseLeave;

                    marker.Shape = shape;
                    MapBrowser.Markers.Add(marker);
                }
            }
            catch( Exception ex )
            {
                MessageBox.Show($"Error loading favorite markers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Marker_MouseEnter(object sender, MouseEventArgs e)
        {
            if(sender is FrameworkElement shape && shape.Tag is string info)
            {
                Point mousePosition = e.GetPosition(this);

                MarkerTooltipText.Text = info;

                MarkerTooltip.Margin = new Thickness(mousePosition.X + 15, mousePosition.Y - 30, 0, 0);
                MarkerTooltip.Visibility = Visibility.Visible;
            }
        }

        private void Marker_MouseLeave(object sender, MouseEventArgs e)
        {
            MarkerTooltip.Visibility = Visibility.Collapsed;
        }
    }
}
