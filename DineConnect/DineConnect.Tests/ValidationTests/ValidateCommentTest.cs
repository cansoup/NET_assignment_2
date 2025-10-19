using NUnit.Framework;
using DineConnect.App.Services.Validation;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidateCommentTests
    {
        /// <summary>
        /// Ensures that validation fails and returns an error when the comment text is null.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsNull()
        {
            var result = ValidateComment.ValidateCreateInput(null);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Comment cannot be empty."));
        }

        /// <summary>
        /// Ensures that validation fails and returns an error when the comment text is empty.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsEmpty()
        {
            var result = ValidateComment.ValidateCreateInput(string.Empty);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Comment cannot be empty."));
        }

        /// <summary>
        /// Ensures that validation fails and returns an error when the comment text is only whitespace.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsWhitespace()
        {
            var result = ValidateComment.ValidateCreateInput("   ");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Comment cannot be empty."));
        }

        /// <summary>
        /// Ensures that validation fails and returns an error when the comment text is too short after trimming.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsTooShort()
        {
            var result = ValidateComment.ValidateCreateInput("ab"); // less than 3 after trim

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Comment must be"));
        }

        /// <summary>
        /// Ensures that validation fails and returns an error when the comment text exceeds the maximum allowed length.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsTooLong()
        {
            var longText = new string('a', 251); // more than 250
            var result = ValidateComment.ValidateCreateInput(longText);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Comment must be"));
        }

        /// <summary>
        /// Ensures that validation passes when the comment text is within the valid length range.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTextIsWithinValidRange()
        {
            var result = ValidateComment.ValidateCreateInput("This is a valid comment.");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        /// <summary>
        /// Ensures that validation trims the comment text before checking its validity.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldTrimTextBeforeValidation()
        {
            var result = ValidateComment.ValidateCreateInput("  ok  "); // becomes "ok" after trim -> invalid

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Comment must be"));
        }

        /// <summary>
        /// Ensures that validation passes when the comment text is exactly the minimum allowed length.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTextIsExactlyMinLength()
        {
            var result = ValidateComment.ValidateCreateInput("abc"); // valid edge case

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        /// <summary>
        /// Ensures that validation passes when the comment text is exactly the maximum allowed length.
        /// </summary>
        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTextIsExactlyMaxLength()
        {
            var text = new string('a', 250);
            var result = ValidateComment.ValidateCreateInput(text);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }
    }
}
