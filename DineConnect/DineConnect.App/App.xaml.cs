using System.Windows;

namespace DineConnect.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create and show the LoginWindow as the first screen.
            var loginWindow = new Views.Auth.LoginWindow();
            // Setting the MainWindow is important for shutdown behavior.
            Current.MainWindow = loginWindow;
            loginWindow.Show();
        }
    }

}
