namespace VortexTrade
{
    public interface IMarketDataService : IDisposable
    {
        Task<IReadOnlyList<TickerInfo>> GetBtcTickersAsync(CancellationToken cancellationToken = default);
    }
}
