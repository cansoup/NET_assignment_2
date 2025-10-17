using NUnit.Framework;
using DineConnect.App.Services.Validation;

namespace DineConnect.Tests.Validation
{
    [TestFixture]
    public class ValidateRestaurantTests
    {
        [Test]
        public void ValidateUpsert_ShouldReturnError_WhenNameIsNullOrWhitespace()
        {
            var resultNull = ValidateRestaurant.ValidateUpsert(null, "Some Address");
            var resultWhitespace = ValidateRestaurant.ValidateUpsert("   ", "Some Address");

            Assert.IsFalse(resultNull.IsValid);
            Assert.That(resultNull.Errors, Does.Contain("Restaurant name is required."));

            Assert.IsFalse(resultWhitespace.IsValid);
            Assert.That(resultWhitespace.Errors, Does.Contain("Restaurant name is required."));
        }

        [Test]
        public void ValidateUpsert_ShouldReturnError_WhenNameIsTooShort()
        {
            var result = ValidateRestaurant.ValidateUpsert("A", "123 Street");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Name must be at least 2 characters."));
        }

        [Test]
        public void ValidateUpsert_ShouldReturnError_WhenNameIsTooLong()
        {
            var longName = new string('a', 121);
            var result = ValidateRestaurant.ValidateUpsert(longName, "123 Street");

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Name must not exceed 120 characters."));
        }

        [Test]
        public void ValidateUpsert_ShouldReturnError_WhenAddressIsTooLong()
        {
            var longAddress = new string('b', 201);
            var result = ValidateRestaurant.ValidateUpsert("Valid Name", longAddress);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors, Does.Contain("Address must not exceed 200 characters."));
        }

        [Test]
        public void ValidateUpsert_ShouldPass_WhenNameIsValidAndAddressIsNull()
        {
            var result = ValidateRestaurant.ValidateUpsert("Good Name", null);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateUpsert_ShouldPass_WhenNameAndAddressAreValid()
        {
            var result = ValidateRestaurant.ValidateUpsert("Nice Restaurant", "123 Main St");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateUpsert_ShouldAcceptEmptyAddressWhenNameIsValid()
        {
            var result = ValidateRestaurant.ValidateUpsert("Valid Name", "");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateUpsert_ShouldPass_WhenNameIsAtBoundaryMinLength()
        {
            var result = ValidateRestaurant.ValidateUpsert("AB", "123 Street");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateUpsert_ShouldPass_WhenNameIsAtBoundaryMaxLength()
        {
            var name = new string('x', 120);
            var result = ValidateRestaurant.ValidateUpsert(name, "123 Street");

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [Test]
        public void ValidateUpsert_ShouldPass_WhenAddressIsAtBoundaryMaxLength()
        {
            var address = new string('y', 200);
            var result = ValidateRestaurant.ValidateUpsert("Valid Name", address);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }
    }
}
