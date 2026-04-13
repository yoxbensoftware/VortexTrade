using System.Globalization;
using System.Text.Json;

namespace VortexTrade
{
    public sealed class CoinLoreMarketDataService : IMarketDataService
    {
        private const string BtcMarketsUrl = "https://api.coinlore.net/api/coin/markets/?id=90";
        private readonly HttpClient _httpClient;

        public CoinLoreMarketDataService()
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        }

        public async Task<IReadOnlyList<TickerInfo>> GetBtcTickersAsync(
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(BtcMarketsUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var tickers = new List<TickerInfo>();

            foreach (var el in doc.RootElement.EnumerateArray())
            {
                var exchange = el.TryGetProperty("name", out var nameProp)
                    ? nameProp.GetString() ?? "?" : "?";
                var baseCurrency = el.TryGetProperty("base", out var baseProp)
                    ? baseProp.GetString() ?? "BTC" : "BTC";
                var quote = el.TryGetProperty("quote", out var quoteProp)
                    ? quoteProp.GetString() ?? "?" : "?";

                var price = GetDecimal(el, "price");
                var usdPrice = GetDecimal(el, "price_usd");
                var volume = GetDecimal(el, "volume");
                var usdVolume = GetDecimal(el, "volume_usd");

                var lastTraded = DateTime.Now;
                if (el.TryGetProperty("time", out var timeProp) &&
                    timeProp.ValueKind == JsonValueKind.Number)
                {
                    var unix = timeProp.GetInt64();
                    lastTraded = DateTimeOffset
                        .FromUnixTimeSeconds(unix)
                        .LocalDateTime;
                }

                tickers.Add(new TickerInfo(
                    Exchange: exchange,
                    Base: baseCurrency,
                    Target: quote,
                    LastPrice: price,
                    UsdPrice: usdPrice,
                    Volume: volume,
                    UsdVolume: usdVolume,
                    SpreadPercent: 0m,
                    TradeUrl: "",
                    LastTraded: lastTraded));
            }

            return tickers
                .OrderByDescending(t => t.UsdVolume)
                .ToList();
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
