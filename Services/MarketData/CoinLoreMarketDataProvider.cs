using System.Globalization;
using System.Text.Json;

namespace VortexTrade
{
    public sealed class CoinLoreMarketDataProvider : IMarketDataProvider
    {
        private const string BaseUrl = "https://api.coinlore.net/api/coin/markets/?id=";
        private const string ProviderLabel = "CoinLore";

        private static readonly Dictionary<string, string> CoinIdMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["BTC"] = "90",
            ["ETH"] = "80",
            ["XRP"] = "58",
            ["SOL"] = "48543"
        };

        private readonly HttpClient _httpClient;

        public CoinLoreMarketDataProvider()
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        }

        public string ProviderName => ProviderLabel;

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    "https://api.coinlore.net/api/global/", cancellationToken);
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
            var resolvedId = CoinIdMap.TryGetValue(coinId, out var mapped) ? mapped : coinId;
            var url = $"{BaseUrl}{resolvedId}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var tickers = new List<MarketTicker>();

            foreach (var el in doc.RootElement.EnumerateArray())
            {
                var exchange = el.TryGetProperty("name", out var nameProp)
                    ? nameProp.GetString() ?? "?" : "?";
                var baseCurrency = el.TryGetProperty("base", out var baseProp)
                    ? baseProp.GetString() ?? coinId.ToUpperInvariant() : coinId.ToUpperInvariant();
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

                tickers.Add(new MarketTicker(
                    Exchange: exchange,
                    BaseCurrency: baseCurrency,
                    QuoteCurrency: quote,
                    Price: price,
                    PriceUsd: usdPrice,
                    Volume: volume,
                    VolumeUsd: usdVolume,
                    LastTraded: lastTraded,
                    Source: ProviderLabel));
            }

            return tickers
                .OrderByDescending(t => t.VolumeUsd)
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
