using System.Collections.Concurrent;
using System.Diagnostics;

namespace VortexTrade
{
    public sealed class TradeScheduler : ITradeScheduler
    {
        private readonly IOrderService _orderService;
        private readonly ConcurrentDictionary<Guid, ScheduledOrderEntry> _orders = new();
        private readonly System.Threading.Timer _timer;
        private bool _disposed;

        public event EventHandler<OrderResult>? OrderExecuted;
        public event EventHandler<ScheduledOrderErrorEventArgs>? OrderFailed;

        public TradeScheduler(IOrderService orderService)
        {
            _orderService = orderService;
            _timer = new System.Threading.Timer(
                CheckScheduledOrders, null,
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        public Guid ScheduleOrder(OrderRequest request, DateTime executeAtUtc)
        {
            var id = Guid.NewGuid();
            _orders.TryAdd(id, new ScheduledOrderEntry(id, request, executeAtUtc));
            return id;
        }

        public bool CancelScheduledOrder(Guid id)
        {
            if (_orders.TryGetValue(id, out var entry) &&
                entry.Status == ScheduledOrderStatus.Pending)
            {
                entry.Status = ScheduledOrderStatus.Cancelled;
                return true;
            }
            return false;
        }

        public IReadOnlyList<ScheduledOrder> GetScheduledOrders()
        {
            return _orders.Values
                .Select(e => new ScheduledOrder(
                    e.Id, e.Request, e.ScheduledTimeUtc, e.Status, e.ErrorMessage))
                .ToList()
                .AsReadOnly();
        }

        private async void CheckScheduledOrders(object? state)
        {
            if (_disposed) return;

            var now = DateTime.UtcNow;
            var pending = _orders.Values
                .Where(o => o.Status == ScheduledOrderStatus.Pending && o.ScheduledTimeUtc <= now)
                .ToList();

            foreach (var entry in pending)
            {
                entry.Status = ScheduledOrderStatus.Executing;
                try
                {
                    var result = await _orderService.ExecuteOrderAsync(entry.Request);
                    entry.Status = result.Success
                        ? ScheduledOrderStatus.Completed
                        : ScheduledOrderStatus.Failed;

                    if (!result.Success)
                        entry.ErrorMessage = result.ErrorMessage;

                    OrderExecuted?.Invoke(this, result);
                }
                catch (Exception ex)
                {
                    entry.Status = ScheduledOrderStatus.Failed;
                    entry.ErrorMessage = ex.Message;
                    OrderFailed?.Invoke(this, new ScheduledOrderErrorEventArgs(entry.Id, ex.Message));
                    Debug.WriteLine($"Scheduled order failed [{entry.Id}]: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _timer.Dispose();
        }

        private sealed class ScheduledOrderEntry(
            Guid id, OrderRequest request, DateTime scheduledTimeUtc)
        {
            public Guid Id { get; } = id;
            public OrderRequest Request { get; } = request;
            public DateTime ScheduledTimeUtc { get; } = scheduledTimeUtc;
            public ScheduledOrderStatus Status { get; set; } = ScheduledOrderStatus.Pending;
            public string? ErrorMessage { get; set; }
        }
    }
}
