namespace VortexTrade
{
    public sealed class MarketDataStream : IMarketDataStream
    {
        private readonly IReadOnlyList<IMarketDataProvider> _providers;
        private readonly SynchronizationContext? _syncContext;

        private CancellationTokenSource? _cts;
        private Task? _pollingTask;
        private IMarketDataProvider? _activeProvider;

        public event EventHandler<MarketDataEventArgs>? DataReceived;
        public event EventHandler<MarketDataErrorEventArgs>? ErrorOccurred;
        public event EventHandler<string>? ProviderChanged;

        public bool IsRunning => _cts is { IsCancellationRequested: false };
        public string? ActiveProviderName => _activeProvider?.ProviderName;

        public MarketDataStream(IEnumerable<IMarketDataProvider> providers)
            : this(providers, SynchronizationContext.Current)
        {
        }

        public MarketDataStream(
            IEnumerable<IMarketDataProvider> providers,
            SynchronizationContext? syncContext)
        {
            _providers = providers.ToList();
            _syncContext = syncContext;

            if (_providers.Count == 0)
                throw new ArgumentException("En az bir provider gereklidir.", nameof(providers));
        }

        public async Task StartAsync(
            string coinId,
            TimeSpan interval,
            CancellationToken cancellationToken = default)
        {
            Stop();

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _activeProvider = await SelectProviderAsync(_cts.Token);

            if (_activeProvider is not null)
                RaiseProviderChanged(_activeProvider.ProviderName);

            _pollingTask = PollAsync(coinId, interval, _cts.Token);
        }

        public void Stop()
        {
            if (_cts is null) return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            _activeProvider = null;
        }

        private async Task PollAsync(string coinId, TimeSpan interval, CancellationToken ct)
        {
            int consecutiveErrors = 0;

            while (!ct.IsCancellationRequested)
            {
                if (_activeProvider is null)
                {
                    _activeProvider = await SelectProviderAsync(ct);
                    if (_activeProvider is null)
                    {
                        await SafeDelay(interval, ct);
                        continue;
                    }
                    RaiseProviderChanged(_activeProvider.ProviderName);
                    consecutiveErrors = 0;
                }

                try
                {
                    var tickers = await _activeProvider.GetTickersAsync(coinId, ct);
                    consecutiveErrors = 0;
                    RaiseDataReceived(tickers, _activeProvider.ProviderName);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    consecutiveErrors++;
                    RaiseErrorOccurred(_activeProvider.ProviderName, ex);

                    if (consecutiveErrors >= 3)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[MarketDataStream] {_activeProvider.ProviderName} üst üste {consecutiveErrors} hata. Fallback deneniyor...");
                        _activeProvider = await SelectProviderAsync(ct, _activeProvider.ProviderName);
                        if (_activeProvider is not null)
                        {
                            RaiseProviderChanged(_activeProvider.ProviderName);
                            consecutiveErrors = 0;
                        }
                    }
                }

                await SafeDelay(interval, ct);
            }
        }

        private async Task<IMarketDataProvider?> SelectProviderAsync(
            CancellationToken ct, string? excludeProvider = null)
        {
            foreach (var provider in _providers)
            {
                if (ct.IsCancellationRequested) break;

                if (excludeProvider is not null &&
                    provider.ProviderName.Equals(excludeProvider, StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    if (await provider.IsAvailableAsync(ct))
                        return provider;
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[MarketDataStream] {provider.ProviderName} erişilebilirlik kontrolü başarısız.");
                }
            }

            // excluded dışında bulunamadıysa, hepsini dene
            if (excludeProvider is not null)
            {
                foreach (var provider in _providers)
                {
                    if (ct.IsCancellationRequested) break;
                    try
                    {
                        if (await provider.IsAvailableAsync(ct))
                            return provider;
                    }
                    catch { }
                }
            }

            return null;
        }

        private static async Task SafeDelay(TimeSpan delay, CancellationToken ct)
        {
            try { await Task.Delay(delay, ct); }
            catch (OperationCanceledException) { }
        }

        private void RaiseDataReceived(IReadOnlyList<MarketTicker> tickers, string providerName)
        {
            var args = new MarketDataEventArgs(tickers, providerName);
            Post(() => DataReceived?.Invoke(this, args));
        }

        private void RaiseErrorOccurred(string providerName, Exception ex)
        {
            var args = new MarketDataErrorEventArgs(providerName, ex);
            Post(() => ErrorOccurred?.Invoke(this, args));
        }

        private void RaiseProviderChanged(string providerName)
        {
            Post(() => ProviderChanged?.Invoke(this, providerName));
        }

        private void Post(Action action)
        {
            if (_syncContext is not null)
                _syncContext.Post(_ => action(), null);
            else
                action();
        }

        public void Dispose()
        {
            Stop();
            foreach (var provider in _providers)
                provider.Dispose();
        }
    }
}
