namespace VortexTrade
{
    public interface IExchange : IDisposable
    {
        ExchangeType ExchangeType { get; }

        Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

        Task<decimal> GetPriceAsync(string symbol, CancellationToken cancellationToken = default);

        Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);
    }
}
