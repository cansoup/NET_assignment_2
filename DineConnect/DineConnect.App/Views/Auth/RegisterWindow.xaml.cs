using DineConnect.App.Services;
using System.Windows;

namespace DineConnect.App.Views.Auth
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService = new AuthService();
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || PasswordBox.Password.Length < 6 || PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Please ensure all fields are correct.\n- Username cannot be empty.\n- Password must be at least 6 characters.\n- Passwords must match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var isSuccess = await _authService.RegisterAsync(UsernameTextBox.Text, PasswordBox.Password);

            if (isSuccess)
            {
                MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show("This username is already taken.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToLoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
