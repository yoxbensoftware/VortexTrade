using System.Globalization;
using System.Text.Json;

namespace VortexTrade
{
    public sealed class CoinGeckoMarketDataService : IMarketDataService
    {
        private const string BaseUrl = "https://api.coingecko.com";
        private readonly HttpClient _httpClient;

        public CoinGeckoMarketDataService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<IReadOnlyList<TickerInfo>> GetBtcTickersAsync(
            CancellationToken cancellationToken = default)
        {
            var tickers = new List<TickerInfo>();
            var page = 1;
            const int perPage = 100;

            // CoinGecko returns max 100 per page, fetch up to 2 pages
            while (page <= 2)
            {
                var url = $"/api/v3/coins/bitcoin/tickers?order=volume_desc&per_page={perPage}&page={page}&depth=false";

                using var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("tickers", out var tickersArray))
                    break;

                var count = 0;
                foreach (var t in tickersArray.EnumerateArray())
                {
                    count++;
                    var exchange = t.TryGetProperty("market", out var market)
                        ? market.GetProperty("name").GetString() ?? "?"
                        : "?";

                    var baseCurrency = t.GetProperty("base").GetString() ?? "BTC";
                    var target = t.GetProperty("target").GetString() ?? "?";

                    var lastPrice = GetDecimal(t, "last");
                    var volume = GetDecimal(t, "volume");

                    var usdPrice = 0m;
                    var usdVolume = 0m;
                    if (t.TryGetProperty("converted_last", out var convLast))
                        usdPrice = GetDecimal(convLast, "usd");
                    if (t.TryGetProperty("converted_volume", out var convVol))
                        usdVolume = GetDecimal(convVol, "usd");

                    var spread = GetDecimal(t, "bid_ask_spread_percentage");

                    var tradeUrl = t.TryGetProperty("trade_url", out var urlProp)
                        ? urlProp.GetString() ?? ""
                        : "";

                    var lastTraded = DateTime.UtcNow;
                    if (t.TryGetProperty("last_traded_at", out var ltProp))
                    {
                        var ltStr = ltProp.GetString();
                        if (DateTime.TryParse(ltStr, CultureInfo.InvariantCulture,
                                DateTimeStyles.RoundtripKind, out var parsed))
                            lastTraded = parsed.ToLocalTime();
                    }

                    tickers.Add(new TickerInfo(
                        Exchange: exchange,
                        Base: baseCurrency,
                        Target: target,
                        LastPrice: lastPrice,
                        UsdPrice: usdPrice,
                        Volume: volume,
                        UsdVolume: usdVolume,
                        SpreadPercent: spread,
                        TradeUrl: tradeUrl,
                        LastTraded: lastTraded));
                }

                if (count < perPage) break;
                page++;
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
