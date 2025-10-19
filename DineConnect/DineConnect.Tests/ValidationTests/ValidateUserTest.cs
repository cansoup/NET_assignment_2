using DineConnect.App.Services.Validation;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidateUserTests
    {
        // === LOGIN TESTS ===

        /// <summary>
        /// Ensures validation fails when the username is missing during login.
        /// </summary>
        [Test]
        public void ValidateLoginInput_ShouldReturnError_WhenUsernameIsMissing()
        {
            var result = ValidateUser.ValidateLoginInput(null, "ValidPass123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Username is required."));
        }

        /// <summary>
        /// Ensures validation fails when the password is missing during login.
        /// </summary>
        [Test]
        public void ValidateLoginInput_ShouldReturnError_WhenPasswordIsMissing()
        {
            var result = ValidateUser.ValidateLoginInput("ValidUser", null);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Password is required."));
        }

        /// <summary>
        /// Ensures validation fails when the username format is invalid during login.
        /// </summary>
        [Test]
        public void ValidateLoginInput_ShouldReturnError_WhenUsernameFormatIsInvalid()
        {
            var result = ValidateUser.ValidateLoginInput("Invalid Username!", "ValidPass123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Username may contain letters, numbers, dots, underscores, and dashes only."));
        }

        /// <summary>
        /// Ensures validation fails when the password hash does not match during login.
        /// </summary>
        [Test]
        public void ValidateLoginInput_ShouldReturnError_WhenPasswordHashDoesNotMatch()
        {
            string password = "ValidPass123";
            string wrongHash = BCrypt.Net.BCrypt.HashPassword("DifferentPass");

            var result = ValidateUser.ValidateLoginInput("User123", password, wrongHash);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Invalid username or password."));
        }

        /// <summary>
        /// Ensures validation passes when the password hash matches during login.
        /// </summary>
        [Test]
        public void ValidateLoginInput_ShouldPass_WhenHashMatches()
        {
            string password = "ValidPass123";
            string correctHash = BCrypt.Net.BCrypt.HashPassword(password);

            var result = ValidateUser.ValidateLoginInput("User123", password, correctHash);

            Assert.IsTrue(result.IsValid);
        }

        // === REGISTRATION TESTS (username/password only) ===

        /// <summary>
        /// Ensures validation fails when the username is invalid during registration.
        /// </summary>
        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenUsernameIsInvalid()
        {
            var result = ValidateUser.ValidateRegistrationInput("ab", "ValidPass123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Username must be 3-32 characters."));
        }

        /// <summary>
        /// Ensures validation fails when the password is too short during registration.
        /// </summary>
        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenPasswordIsTooShort()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "Short1");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Password must be at least 8 characters."));
        }

        /// <summary>
        /// Ensures validation fails when the password lacks a lowercase letter during registration.
        /// </summary>
        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenPasswordLacksLowercase()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "PASSWORD123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Password must contain a lowercase letter."));
        }

        /// <summary>
        /// Ensures validation fails when the password lacks an uppercase letter during registration.
        /// </summary>
        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenPasswordLacksUppercase()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "password123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Password must contain an uppercase letter."));
        }

        /// <summary>
        /// Ensures validation passes when both username and password are valid during registration.
        /// </summary>
        [Test]
        public void ValidateRegistrationInput_ShouldPass_WhenUsernameAndPasswordAreValid()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "ValidPass123");

            Assert.IsTrue(result.IsValid);
        }

        // === REGISTRATION WITH CONFIRM PASSWORD ===

        /// <summary>
        /// Ensures validation fails when the passwords do not match during registration.
        /// </summary>
        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenPasswordsDoNotMatch()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "ValidPass123", "DifferentPass");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Passwords must match."));
        }

        /// <summary>
        /// Ensures validation passes when the passwords match during registration.
        /// </summary>
        [Test]
        public void ValidateRegistrationInput_ShouldPass_WhenPasswordsMatch()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "ValidPass123", "ValidPass123");

            Assert.IsTrue(result.IsValid);
        }
    }
}
