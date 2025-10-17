using NUnit.Framework;
using DineConnect.App.Services.Validation;
using System;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidateReservationTests
    {
        [Test]
        public void ValidateCreate_ShouldReturnErrors_WhenAllInputsAreInvalid()
        {
            var pastDate = DateTime.Now.AddHours(-1);
            var result = ValidateReservation.ValidateCreate(null, pastDate, null);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("A restaurant must be selected."));
            Assert.That(result.Errors, Does.Contain("Please select a valid future date & time."));
            Assert.That(result.Errors, Does.Contain("Party size must be between 1 and 12."));
        }

        [Test]
        public void ValidateCreate_ShouldReturnError_WhenRestaurantIdIsInvalid()
        {
            var futureDate = DateTime.Now.AddHours(1);
            var result = ValidateReservation.ValidateCreate(0, futureDate, 4);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("A restaurant must be selected."));
        }

        [Test]
        public void ValidateCreate_ShouldReturnError_WhenDateIsNullOrPast()
        {
            var pastDate = DateTime.Now.AddHours(-2);
            var resultWithNullDate = ValidateReservation.ValidateCreate(1, null, 4);
            var resultWithPastDate = ValidateReservation.ValidateCreate(1, pastDate, 4);

            Assert.IsFalse(resultWithNullDate.IsValid);
            Assert.IsFalse(resultWithPastDate.IsValid);
            Assert.That(resultWithNullDate.Errors, Does.Contain("Please select a valid future date & time."));
            Assert.That(resultWithPastDate.Errors, Does.Contain("Please select a valid future date & time."));
        }

        [TestCase(null)]
        [TestCase(0)]
        [TestCase(13)]
        [TestCase(-1)]
        public void ValidateCreate_ShouldReturnError_WhenPartySizeIsInvalid(int? invalidPartySize)
        {
            var futureDate = DateTime.Now.AddHours(1);
            var result = ValidateReservation.ValidateCreate(1, futureDate, invalidPartySize);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Party size must be between 1 and 12."));
        }

        [Test]
        public void ValidateCreate_ShouldPass_WhenAllInputsAreValid()
        {
            var futureDate = DateTime.Now.AddHours(2);
            var result = ValidateReservation.ValidateCreate(5, futureDate, 4);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateCreate_ShouldPass_WhenPartySizeIsAtMinBoundary()
        {
            var futureDate = DateTime.Now.AddHours(1);
            var result = ValidateReservation.ValidateCreate(1, futureDate, 1);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateCreate_ShouldPass_WhenPartySizeIsAtMaxBoundary()
        {
            var futureDate = DateTime.Now.AddHours(1);
            var result = ValidateReservation.ValidateCreate(1, futureDate, 12);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }
    }
}
