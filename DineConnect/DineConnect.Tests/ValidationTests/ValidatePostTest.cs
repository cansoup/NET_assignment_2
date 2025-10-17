using NUnit.Framework;
using DineConnect.App.Services.Validation;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidatePostTests
    {
        [Test]
        public void ValidateCreateInput_ShouldReturnErrors_WhenTitleAndContentAreNull()
        {
            var result = ValidatePost.ValidateCreateInput(null, null);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Title is required."));
            Assert.That(result.Errors, Does.Contain("Content is required."));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTitleIsTooShort()
        {
            var result = ValidatePost.ValidateCreateInput("abcd", "Valid content here");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Title must be"));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTitleIsTooLong()
        {
            var longTitle = new string('a', 121);
            var result = ValidatePost.ValidateCreateInput(longTitle, "Valid content here");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Title must be"));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTitleIsOnlyWhitespace()
        {
            var result = ValidatePost.ValidateCreateInput("     ", "Valid content");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Title is required."));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenContentIsTooShort()
        {
            var result = ValidatePost.ValidateCreateInput("Valid Title", "abcd");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Content must be"));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenContentIsTooLong()
        {
            var longContent = new string('a', 301);
            var result = ValidatePost.ValidateCreateInput("Valid Title", longContent);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Content must be"));
        }

        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTitleAndContentAreValid()
        {
            var result = ValidatePost.ValidateCreateInput("Valid Title", "This is valid content.");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTitleAndContentAreExactMinLength()
        {
            var result = ValidatePost.ValidateCreateInput("12345", "12345");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTitleAndContentAreExactMaxLength()
        {
            var title = new string('a', 120);
            var content = new string('a', 300);

            var result = ValidatePost.ValidateCreateInput(title, content);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }
    }
}
