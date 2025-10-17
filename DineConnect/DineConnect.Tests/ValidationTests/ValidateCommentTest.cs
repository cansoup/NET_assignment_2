using NUnit.Framework;
using DineConnect.App.Services.Validation;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidateCommentTests
    {
        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsNull()
        {
            var result = ValidateComment.ValidateCreateInput(null);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Comment cannot be empty."));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsEmpty()
        {
            var result = ValidateComment.ValidateCreateInput(string.Empty);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Comment cannot be empty."));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsWhitespace()
        {
            var result = ValidateComment.ValidateCreateInput("   ");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Comment cannot be empty."));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsTooShort()
        {
            var result = ValidateComment.ValidateCreateInput("ab"); // less than 3 after trim

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Comment must be"));
        }

        [Test]
        public void ValidateCreateInput_ShouldReturnError_WhenTextIsTooLong()
        {
            var longText = new string('a', 251); // more than 250
            var result = ValidateComment.ValidateCreateInput(longText);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Comment must be"));
        }

        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTextIsWithinValidRange()
        {
            var result = ValidateComment.ValidateCreateInput("This is a valid comment.");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateCreateInput_ShouldTrimTextBeforeValidation()
        {
            var result = ValidateComment.ValidateCreateInput("  ok  "); // becomes "ok" after trim -> invalid

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Comment must be"));
        }

        [Test]
        public void ValidateCreateInput_ShouldPass_WhenTextIsExactlyMinLength()
        {
            var result = ValidateComment.ValidateCreateInput("abc"); // valid edge case

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

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
