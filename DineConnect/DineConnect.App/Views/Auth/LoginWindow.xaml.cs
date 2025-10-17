using DineConnect.App.Services;
using System.Windows;

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
                DialogResult = true; // closes login and returns true to App.xaml.cs
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow { Owner = this };
            if (registerWindow.ShowDialog() == true)
            {
                UsernameTextBox.Text = registerWindow.RegisteredUsername;
            }

        }
    }
}
