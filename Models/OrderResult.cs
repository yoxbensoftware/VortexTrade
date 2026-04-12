namespace VortexTrade
{
    public sealed record OrderResult(
        bool Success,
        string? OrderId,
        string Symbol,
        OrderSide Side,
        decimal ExecutedQuantity,
        decimal ExecutedPrice,
        DateTime Timestamp,
        string? ErrorMessage = null);
}
