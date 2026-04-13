namespace VortexTrade
{
    public interface IMarketDataProvider : IDisposable
    {
        string ProviderName { get; }

        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MarketTicker>> GetTickersAsync(
            string coinId,
            CancellationToken cancellationToken = default);
    }
}
