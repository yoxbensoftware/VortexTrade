using System.Globalization;
using System.Text.Json;

namespace VortexTrade
{
    public sealed class BinanceMarketDataService : IMarketDataService
    {
        private const string BaseUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;

        public BinanceMarketDataService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<IReadOnlyList<TickerInfo>> GetBtcTickersAsync(
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(
                "/api/v3/ticker/24hr", cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var tickers = new List<TickerInfo>();

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                var symbol = element.GetProperty("symbol").GetString() ?? "";

                if (!symbol.Contains("BTC", StringComparison.OrdinalIgnoreCase))
                    continue;

                tickers.Add(new TickerInfo(
                    Symbol: symbol,
                    LastPrice: ParseDecimal(element, "lastPrice"),
                    PriceChangePercent: ParseDecimal(element, "priceChangePercent"),
                    HighPrice: ParseDecimal(element, "highPrice"),
                    LowPrice: ParseDecimal(element, "lowPrice"),
                    Volume: ParseDecimal(element, "volume"),
                    QuoteVolume: ParseDecimal(element, "quoteVolume")));
            }

            return tickers
                .OrderByDescending(t => t.QuoteVolume)
                .ToList();
        }

        private static decimal ParseDecimal(JsonElement element, string property)
        {
            var str = element.GetProperty(property).GetString();
            return decimal.TryParse(str, CultureInfo.InvariantCulture, out var value)
                ? value
                : 0m;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
