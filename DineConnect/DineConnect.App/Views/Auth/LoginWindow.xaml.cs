using DineConnect.Services;
using System.Windows;
using System.Windows.Controls;

namespace DineConnect.App.Views.Auth
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService = new AuthService();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var isSuccess = await _authService.LoginAsync(UsernameTextBox.Text, PasswordBox.Password);

            if (isSuccess)
            {
                // if login is successful, open the main window and close the login window
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
