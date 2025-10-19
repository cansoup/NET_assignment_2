using NUnit.Framework;
using DineConnect.App.Services.Validation;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidateRatingTests
    {
        /// <summary>
        /// Ensures validation fails and returns an error when the rating is out of the allowed range.
        /// </summary>
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

        /// <summary>
        /// Ensures validation passes when the rating is within the valid range.
        /// </summary>
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void Validate_ShouldPass_WhenRatingIsWithinValidRange(int validRating)
        {
            var result = ValidateRating.Validate(validRating);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        /// <summary>
        /// Ensures validation fails when the rating is exactly below the minimum allowed value.
        /// </summary>
        [Test]
        public void Validate_ShouldReturnError_WhenRatingIsExactlyBelowMin()
        {
            var result = ValidateRating.Validate(0);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Rating must be between"));
        }

        /// <summary>
        /// Ensures validation fails when the rating is exactly above the maximum allowed value.
        /// </summary>
        [Test]
        public void Validate_ShouldReturnError_WhenRatingIsExactlyAboveMax()
        {
            var result = ValidateRating.Validate(6);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0], Does.Contain("Rating must be between"));
        }

        /// <summary>
        /// Ensures validation passes when the rating is exactly at the minimum allowed value.
        /// </summary>
        [Test]
        public void Validate_ShouldPass_WhenRatingIsExactlyMin()
        {
            var result = ValidateRating.Validate(1);

            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Ensures validation passes when the rating is exactly at the maximum allowed value.
        /// </summary>
        [Test]
        public void Validate_ShouldPass_WhenRatingIsExactlyMax()
        {
            var result = ValidateRating.Validate(5);

            Assert.IsTrue(result.IsValid);
        }
    }
}
