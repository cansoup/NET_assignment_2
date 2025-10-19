namespace DineConnect.App.Services.Validation
{
    /// <summary>
    /// Represents the result of a validation operation, including validity and error messages.
    /// </summary>
    public sealed class ValidationResult
    {
        private readonly List<string> _errors = new();

        public bool IsValid => _errors.Count == 0;
        public IReadOnlyList<string> Errors => _errors;

        public void AddError(string message) => _errors.Add(message);
    }
}
