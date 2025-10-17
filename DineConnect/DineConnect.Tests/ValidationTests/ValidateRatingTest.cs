using NUnit.Framework;
using DineConnect.App.Services.Validation;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidateRatingTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(6)]
        [TestCase(10)]
        public void Validate_ShouldReturnError_WhenRatingIsOutOfRange(int invalidRating)
        {
            var result = ValidateRating.Validate(invalidRating);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Rating must be between"));
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void Validate_ShouldPass_WhenRatingIsWithinValidRange(int validRating)
        {
            var result = ValidateRating.Validate(validRating);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void Validate_ShouldReturnError_WhenRatingIsExactlyBelowMin()
        {
            var result = ValidateRating.Validate(0);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Rating must be between"));
        }

        [Test]
        public void Validate_ShouldReturnError_WhenRatingIsExactlyAboveMax()
        {
            var result = ValidateRating.Validate(6);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Rating must be between"));
        }

        [Test]
        public void Validate_ShouldPass_WhenRatingIsExactlyMin()
        {
            var result = ValidateRating.Validate(1);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void Validate_ShouldPass_WhenRatingIsExactlyMax()
        {
            var result = ValidateRating.Validate(5);

            Assert.IsTrue(result.IsValid);
        }
    }
}
