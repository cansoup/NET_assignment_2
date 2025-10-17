namespace DineConnect.App.Services.Validation
{
    public static class ValidateRating
    {
        private const int MinRating = 1;
        private const int MaxRating = 5;

        public static ValidationResult Validate(int rating)
        {
            var result = new ValidationResult();

            if (rating < MinRating || rating > MaxRating)
                result.AddError($"Rating must be between {MinRating} and {MaxRating}.");

            return result;
        }
    }
}
