using NUnit.Framework;
using DineConnect.App.Services.Validation;
using System;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidateUserTests
    {
        // === LOGIN TESTS ===

        [Test]
        public void ValidateLoginInput_ShouldReturnError_WhenUsernameIsMissing()
        {
            var result = ValidateUser.ValidateLoginInput(null, "ValidPass123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Username is required."));
        }

        [Test]
        public void ValidateLoginInput_ShouldReturnError_WhenPasswordIsMissing()
        {
            var result = ValidateUser.ValidateLoginInput("ValidUser", null);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Password is required."));
        }

        [Test]
        public void ValidateLoginInput_ShouldReturnError_WhenUsernameFormatIsInvalid()
        {
            var result = ValidateUser.ValidateLoginInput("Invalid Username!", "ValidPass123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Username may contain letters, numbers, dots, underscores, and dashes only."));
        }

        [Test]
        public void ValidateLoginInput_ShouldReturnError_WhenPasswordHashDoesNotMatch()
        {
            string password = "ValidPass123";
            string wrongHash = BCrypt.Net.BCrypt.HashPassword("DifferentPass");

            var result = ValidateUser.ValidateLoginInput("User123", password, wrongHash);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Invalid username or password."));
        }

        [Test]
        public void ValidateLoginInput_ShouldPass_WhenHashMatches()
        {
            string password = "ValidPass123";
            string correctHash = BCrypt.Net.BCrypt.HashPassword(password);

            var result = ValidateUser.ValidateLoginInput("User123", password, correctHash);

            Assert.IsTrue(result.IsValid);
        }

        // === REGISTRATION TESTS (username/password only) ===

        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenUsernameIsInvalid()
        {
            var result = ValidateUser.ValidateRegistrationInput("ab", "ValidPass123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Username must be 3-32 characters."));
        }

        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenPasswordIsTooShort()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "Short1");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Password must be at least 8 characters."));
        }

        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenPasswordLacksLowercase()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "PASSWORD123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Password must contain a lowercase letter."));
        }

        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenPasswordLacksUppercase()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "password123");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Password must contain an uppercase letter."));
        }

        [Test]
        public void ValidateRegistrationInput_ShouldPass_WhenUsernameAndPasswordAreValid()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "ValidPass123");

            Assert.IsTrue(result.IsValid);
        }

        // === REGISTRATION WITH CONFIRM PASSWORD ===

        [Test]
        public void ValidateRegistrationInput_ShouldReturnError_WhenPasswordsDoNotMatch()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "ValidPass123", "DifferentPass");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Passwords must match."));
        }

        [Test]
        public void ValidateRegistrationInput_ShouldPass_WhenPasswordsMatch()
        {
            var result = ValidateUser.ValidateRegistrationInput("ValidUser", "ValidPass123", "ValidPass123");

            Assert.IsTrue(result.IsValid);
        }
    }
}
