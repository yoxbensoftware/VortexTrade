namespace VortexTrade
{
    public interface ITradeScheduler : IDisposable
    {
        Guid ScheduleOrder(OrderRequest request, DateTime executeAtUtc);

        bool CancelScheduledOrder(Guid id);

        IReadOnlyList<ScheduledOrder> GetScheduledOrders();

        event EventHandler<OrderResult>? OrderExecuted;

        event EventHandler<ScheduledOrderErrorEventArgs>? OrderFailed;
    }

    public sealed class ScheduledOrderErrorEventArgs(
        Guid scheduledOrderId, string errorMessage) : EventArgs
    {
        public Guid ScheduledOrderId { get; } = scheduledOrderId;
        public string ErrorMessage { get; } = errorMessage;
    }
}
