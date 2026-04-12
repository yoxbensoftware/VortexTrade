namespace VortexTrade
{
    public sealed record ScheduledOrder(
        Guid Id,
        OrderRequest Request,
        DateTime ScheduledTimeUtc,
        ScheduledOrderStatus Status,
        string? ErrorMessage = null);
}
