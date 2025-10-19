namespace DineConnect.App.Services.Validation
{
    /// <summary>
    /// Provides validation logic for reservations, including restaurant, date/time, and party size checks.
    /// </summary>
    public static class ValidateReservation
    {
        private const int MinPartySize = 1;
        private const int MaxPartySize = 12;

        public static ValidationResult ValidateCreate(int? restaurantId, DateTime? at, int? partySize)
        {
            var result = new ValidationResult();

            // Restaurant must be selected
            if (restaurantId is null || restaurantId <= 0)
                result.AddError("A restaurant must be selected.");

            // Date/time must be valid & in the future
            if (at is null || at <= DateTime.Now)
                result.AddError("Please select a valid future date & time.");

            // Party size must be valid
            if (partySize is null || partySize < MinPartySize || partySize > MaxPartySize)
                result.AddError($"Party size must be between {MinPartySize} and {MaxPartySize}.");

            return result;
        }
    }
}
