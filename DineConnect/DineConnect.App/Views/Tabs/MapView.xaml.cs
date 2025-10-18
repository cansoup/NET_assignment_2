using GMap.NET;
using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace DineConnect.App.Views.Tabs
{
    /// <summary>
    /// MapView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();

            Loaded += MapView_Loaded;
        }

        private void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            MapBrowser.MapProvider = GMapProviders.GoogleMap;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            MapBrowser.DragButton = MouseButton.Left;

            MapBrowser.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            MapBrowser.MinZoom = 2;
            MapBrowser.MaxZoom = 19;
            MapBrowser.Zoom = 15;

            MapBrowser.Position = new GMap.NET.PointLatLng(-33.8834, 151.2069);

            MapBrowser.ShowCenter = false;
        }
    }
}
