using System.Globalization;
using System.Text.Json;

namespace VortexTrade
{
    public sealed class BinanceMarketDataProvider : IMarketDataProvider
    {
        private const string BaseUrl = "https://api.binance.com";
        private const string TickerEndpoint = "/api/v3/ticker/24hr";
        private const string ProviderLabel = "Binance";

        private readonly HttpClient _httpClient;

        public BinanceMarketDataProvider()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public string ProviderName => ProviderLabel;

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    "/api/v3/ping", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IReadOnlyList<MarketTicker>> GetTickersAsync(
            string coinId,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(TickerEndpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var tickers = new List<MarketTicker>();

            foreach (var el in doc.RootElement.EnumerateArray())
            {
                var symbol = el.GetProperty("symbol").GetString() ?? "";

                if (!IsBtcPair(symbol))
                    continue;

                var (baseCurrency, quoteCurrency) = SplitSymbol(symbol);
                var lastPrice = GetDecimal(el, "lastPrice");
                var volume = GetDecimal(el, "volume");
                var quoteVolume = GetDecimal(el, "quoteVolume");

                var priceUsd = quoteCurrency.Equals("USDT", StringComparison.OrdinalIgnoreCase)
                    || quoteCurrency.Equals("USD", StringComparison.OrdinalIgnoreCase)
                    ? lastPrice
                    : 0m;

                tickers.Add(new MarketTicker(
                    Exchange: "Binance",
                    BaseCurrency: baseCurrency,
                    QuoteCurrency: quoteCurrency,
                    Price: lastPrice,
                    PriceUsd: priceUsd,
                    Volume: volume,
                    VolumeUsd: quoteVolume,
                    LastTraded: DateTime.Now,
                    Source: ProviderLabel));
            }

            return tickers
                .OrderByDescending(t => t.VolumeUsd)
                .ToList();
        }

        private static bool IsBtcPair(string symbol)
        {
            return symbol.StartsWith("BTC", StringComparison.OrdinalIgnoreCase)
                || symbol.EndsWith("BTC", StringComparison.OrdinalIgnoreCase);
        }

        private static (string Base, string Quote) SplitSymbol(string symbol)
        {
            string[] quoteAssets = ["USDT", "BUSD", "USDC", "TUSD", "EUR", "GBP", "TRY", "BRL", "ARS", "BTC"];

            foreach (var quote in quoteAssets)
            {
                if (symbol.EndsWith(quote, StringComparison.OrdinalIgnoreCase))
                {
                    var basePart = symbol[..^quote.Length];
                    if (basePart.Length > 0)
                        return (basePart, quote);
                }
            }

            return symbol.StartsWith("BTC", StringComparison.OrdinalIgnoreCase)
                ? ("BTC", symbol[3..])
                : (symbol, "?");
        }

        private static decimal GetDecimal(JsonElement element, string property)
        {
            if (!element.TryGetProperty(property, out var prop))
                return 0m;

            return prop.ValueKind switch
            {
                JsonValueKind.Number => prop.GetDecimal(),
                JsonValueKind.String => decimal.TryParse(
                    prop.GetString(), CultureInfo.InvariantCulture, out var v) ? v : 0m,
                _ => 0m
            };
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
