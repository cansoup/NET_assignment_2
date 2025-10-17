using System.Text.RegularExpressions;

namespace DineConnect.App.Services.Validation
{
    public sealed class ValidateUser
    {
        private const int MinUsernameLength = 3;
        private const int MaxUsernameLength = 32;
        private const int MinPasswordLength = 8;

        public static ValidationResult ValidateLoginInput(string? username, string? password, string? storedHash = null)
        {
            var result = new ValidationResult();

            ValidateUsernameCommon(username, result);
            ValidatePasswordProvided(password, result);

            // Hash check only if hash is provided (AuthService will pass it)
            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(storedHash))
            {
                bool verified = BCrypt.Net.BCrypt.Verify(password, storedHash);
                if (!verified)
                    result.AddError("Invalid username or password.");
            }

            return result;
        }

        public static ValidationResult ValidateRegistrationInput(string? username, string? password)
        {
            var result = new ValidationResult();

            ValidateUsernameCommon(username, result);
            ValidatePasswordStrength(password, result);

            return result;
        }

        public static ValidationResult ValidateRegistrationInput(
            string? username,
            string? password,
            string? confirmPassword)
        {
            var result = ValidateRegistrationInput(username, password);

            if (!string.IsNullOrWhiteSpace(password) &&
                !string.IsNullOrWhiteSpace(confirmPassword) &&
                password.Trim() != confirmPassword.Trim())
            {
                result.AddError("Passwords must match.");
            }

            return result;
        }

        // === Internal helpers ===
        private static void ValidateUsernameCommon(string? username, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                result.AddError("Username is required.");
                return;
            }

            var trimmed = username.Trim();

            if (trimmed.Length < MinUsernameLength || trimmed.Length > MaxUsernameLength)
                result.AddError($"Username must be {MinUsernameLength}-{MaxUsernameLength} characters.");

            if (!Regex.IsMatch(trimmed, @"^[A-Za-z0-9._-]+$"))
                result.AddError("Username may contain letters, numbers, dots, underscores, and dashes only.");
        }

        private static void ValidatePasswordProvided(string? password, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(password))
                result.AddError("Password is required.");
        }

        private static void ValidatePasswordStrength(string? password, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                result.AddError("Password is required.");
                return;
            }

            var p = password.Trim();

            if (p.Length < MinPasswordLength)
                result.AddError($"Password must be at least {MinPasswordLength} characters.");
            if (!Regex.IsMatch(p, @"[a-z]")) result.AddError("Password must contain a lowercase letter.");
            if (!Regex.IsMatch(p, @"[A-Z]")) result.AddError("Password must contain an uppercase letter.");
        }
    }

    public sealed class ValidationResult
    {
        private readonly List<string> _errors = new();

        public bool IsValid => _errors.Count == 0;
        public IReadOnlyList<string> Errors => _errors;

        public void AddError(string message) => _errors.Add(message);
    }
}
