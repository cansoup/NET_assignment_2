using DineConnect.App.Services;
using System.Windows;

namespace DineConnect.App.Views.Auth
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService = new AuthService();

        // Optional: lets the LoginWindow prefill the username after a successful registration
        public string RegisteredUsername { get; private set; }

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text)
                || PasswordBox.Password.Length < 6
                || PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show(
                    "Please ensure all fields are correct.\n- Username cannot be empty.\n- Password must be at least 6 characters.\n- Passwords must match.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var isSuccess = await _authService.RegisterAsync(UsernameTextBox.Text, PasswordBox.Password);

            if (isSuccess)
            {
                // capture for caller (LoginWindow) to optionally prefill
                RegisteredUsername = UsernameTextBox.Text;

                MessageBox.Show("Registration successful! You can now log in.",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // IMPORTANT: return success to the caller; DO NOT create/show MainWindow here
                DialogResult = true; // this also closes the dialog
            }
            else
            {
                MessageBox.Show("This username is already taken.",
                                "Registration Failed",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BackToLoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            // Return control to the LoginWindow without side effects
            DialogResult = false; // this also closes the dialog
        }
    }
}
