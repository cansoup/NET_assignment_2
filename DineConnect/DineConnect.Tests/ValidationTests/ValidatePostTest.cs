using DineConnect.App.Services.Validation;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidatePostTests
    {
        /// <summary>
        /// Ensures validation fails and returns errors when both title and content are null.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnErrors_WhenTitleAndContentAreNull()
        {
            var result = ValidatePost.ValidateCreateInput(null, null);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Title is required."));
            Assert.That(result.Errors, Does.Contain("Content is required."));
        }

        /// <summary>
        /// Ensures validation fails when the title is shorter than the minimum allowed length.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTitleIsTooShort()
        {
            var result = ValidatePost.ValidateCreateInput("abcd", "Valid content here");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Title must be"));
        }

        /// <summary>
        /// Ensures validation fails when the title exceeds the maximum allowed length.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTitleIsTooLong()
        {
            var longTitle = new string('a', 121);
            var result = ValidatePost.ValidateCreateInput(longTitle, "Valid content here");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Title must be"));
        }

        /// <summary>
        /// Ensures validation fails when the title is only whitespace.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTitleIsOnlyWhitespace()
        {
            var result = ValidatePost.ValidateCreateInput("     ", "Valid content");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Title is required."));
        }

        /// <summary>
        /// Ensures validation fails when the content is shorter than the minimum allowed length.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenContentIsTooShort()
        {
            var result = ValidatePost.ValidateCreateInput("Valid Title", "abcd");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Content must be"));
        }

        /// <summary>
        /// Ensures validation fails when the content exceeds the maximum allowed length.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenContentIsTooLong()
        {
            var longContent = new string('a', 301);
            var result = ValidatePost.ValidateCreateInput("Valid Title", longContent);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Content must be"));
        }

        /// <summary>
        /// Ensures validation passes when both title and content are valid.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTitleAndContentAreValid()
        {
            var result = ValidatePost.ValidateCreateInput("Valid Title", "This is valid content.");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        /// <summary>
        /// Ensures validation passes when both title and content are exactly at the minimum allowed length.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTitleAndContentAreExactMinLength()
        {
            var result = ValidatePost.ValidateCreateInput("12345", "12345");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        /// <summary>
        /// Ensures validation passes when both title and content are exactly at the maximum allowed length.
        /// </summary>
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
