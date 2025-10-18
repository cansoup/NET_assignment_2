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

            MapBrowser.DragButton = MouseButton.Left;
            MapBrowser.MapProvider = GMapProviders.GoogleMap;
            MapBrowser.Position = new GMap.NET.PointLatLng(-33.8834, 151.2069);
            MapBrowser.Zoom = 19;
        }
    }
}
