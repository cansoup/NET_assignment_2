using DineConnect.App.Views;
using System.Windows;
using System.Windows.Controls;

namespace DineConnect.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set the starting page to be MyFavoritesView
            MainFrame.Content = new MyFavoritesView();
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            // Navigate to the correct view based on which button was clicked
            switch (button.Name)
            {
                case "MyFavoritesButton":
                    MainFrame.Navigate(new MyFavoritesView());
                    break;
                //case "CommunityPicksButton":
                //    MainFrame.Navigate(new CommunityPicksView());
                //    break;
            }
        }
    }
}