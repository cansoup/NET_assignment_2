using DineConnect.App.Services.Validation;
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
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;
            var confirm = ConfirmPasswordBox.Password;

            var validation = ValidateUser.ValidateRegistrationInput(username, password, confirm);

            if (!validation.IsValid)
            {
                MessageBox.Show(
                    string.Join("\n", validation.Errors),
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var isSuccess = await _authService.RegisterAsync(username, password);

            if (isSuccess)
            {
                RegisteredUsername = username;

                MessageBox.Show("Registration successful! You can now log in.",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // IMPORTANT: return success to the caller; DO NOT create/show MainWindow here
                DialogResult = true; // this also closes the dialog
            }
            else
            {
                // Service side currently returns false when the username already exists
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
