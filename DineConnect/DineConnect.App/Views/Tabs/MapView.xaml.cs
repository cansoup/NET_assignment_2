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
            LoadMap();
        }

        private void LoadMap()
        {
            try
            {
                string? exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (exePath == null) return;

                string htmlPath = Path.Combine(exePath, "map.html");

                if (File.Exists(htmlPath))
                {
                    MapBrowser.Navigate(new Uri(htmlPath));
                }
                else
                {
                    MessageBox.Show("map.html file not found in the execution directory.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading map: {ex.Message}", "Map Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
