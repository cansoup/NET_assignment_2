namespace DineConnect.App.Services.Validation
{
    public static class ValidateComment
    {
        // tweak as desired
        private const int MinCommentLength = 3;
        private const int MaxCommentLength = 250;

        public static ValidationResult ValidateCreateInput(string? text)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(text))
            {
                result.AddError("Comment cannot be empty.");
            }
            else
            {
                var t = text.Trim();
                if (t.Length < MinCommentLength || t.Length > MaxCommentLength)
                    result.AddError($"Comment must be {MinCommentLength}-{MaxCommentLength} characters.");
            }

            return result;
        }
    }
}
