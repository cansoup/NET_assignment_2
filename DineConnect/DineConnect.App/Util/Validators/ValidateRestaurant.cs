namespace DineConnect.App.Services.Validation
{
    public static class ValidateRestaurant
    {
        private const int MinNameLength = 2;
        private const int MaxNameLength = 120;
        private const int MaxAddressLength = 200;

        public static ValidationResult ValidateUpsert(string? name, string? address)
        {
            var result = new ValidationResult();

            // Name (required)
            if (string.IsNullOrWhiteSpace(name))
            {
                result.AddError("Restaurant name is required.");
            }
            else
            {
                var n = name.Trim();
                if (n.Length < MinNameLength)
                    result.AddError($"Name must be at least {MinNameLength} characters.");
                if (n.Length > MaxNameLength)
                    result.AddError($"Name must not exceed {MaxNameLength} characters.");
            }

            // Address (optional but bounded)
            if (!string.IsNullOrWhiteSpace(address))
            {
                var a = address.Trim();
                if (a.Length > MaxAddressLength)
                    result.AddError($"Address must not exceed {MaxAddressLength} characters.");
            }

            return result;
        }
    }
}
