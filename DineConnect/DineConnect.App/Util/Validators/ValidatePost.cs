using System.Text.RegularExpressions;

namespace DineConnect.App.Services.Validation
{
    /// <summary>
    /// Provides validation logic for user posts, including title and content checks.
    /// </summary>
    public static class ValidatePost
    {
        private const int MinTitleLength = 5;
        private const int MaxTitleLength = 120;
        private const int MinContentLength = 5;
        private const int MaxContentLength = 300;

        public static ValidationResult ValidateCreateInput(string? title, string? content)
        {
            var result = new ValidationResult();

            // Title
            if (string.IsNullOrWhiteSpace(title))
            {
                result.AddError("Title is required.");
            }
            else
            {
                var t = title.Trim();
                if (t.Length < MinTitleLength || t.Length > MaxTitleLength)
                    result.AddError($"Title must be {MinTitleLength}-{MaxTitleLength} characters.");
                if (!Regex.IsMatch(t, @"\S"))
                    result.AddError("Title cannot be only whitespace.");
            }

            // Content
            if (string.IsNullOrWhiteSpace(content))
            {
                result.AddError("Content is required.");
            }
            else
            {
                var c = content.Trim();
                if (c.Length < MinContentLength || c.Length > MaxContentLength)
                    result.AddError($"Content must be {MinContentLength}-{MaxContentLength} characters.");
            }

            return result;
        }
    }
}
