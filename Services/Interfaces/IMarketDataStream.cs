namespace VortexTrade
{
    public sealed class MarketDataEventArgs(IReadOnlyList<MarketTicker> tickers, string providerName)
        : EventArgs
    {
        public IReadOnlyList<MarketTicker> Tickers { get; } = tickers;
        public string ProviderName { get; } = providerName;
    }

    public sealed class MarketDataErrorEventArgs(string providerName, Exception exception) : EventArgs
    {
        public string ProviderName { get; } = providerName;
        public Exception Exception { get; } = exception;
    }

    public interface IMarketDataStream : IDisposable
    {
        event EventHandler<MarketDataEventArgs>? DataReceived;
        event EventHandler<MarketDataErrorEventArgs>? ErrorOccurred;
        event EventHandler<string>? ProviderChanged;

        bool IsRunning { get; }
        string? ActiveProviderName { get; }

        Task StartAsync(string coinId, TimeSpan interval, CancellationToken cancellationToken = default);
        void Stop();
    }
}
