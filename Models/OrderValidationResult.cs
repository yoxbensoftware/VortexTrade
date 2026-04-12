namespace VortexTrade
{
    public sealed record OrderValidationResult(
        bool IsValid,
        IReadOnlyList<string> Errors)
    {
        public static OrderValidationResult Valid() =>
            new(true, Array.Empty<string>());

        public static OrderValidationResult WithErrors(IReadOnlyList<string> errors) =>
            new(false, errors);
    }
}
