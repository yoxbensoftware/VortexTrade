using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace VortexTrade
{
    public sealed class BinanceExchange : IExchange
    {
        private const string BaseUrl = "https://api.binance.com";
        private readonly HttpClient _httpClient;
        private readonly byte[] _secretKeyBytes;

        public ExchangeType ExchangeType => ExchangeType.Binance;

        public BinanceExchange(ExchangeCredentials credentials)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(credentials.ApiKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(credentials.ApiSecret);

            _secretKeyBytes = Encoding.UTF8.GetBytes(credentials.ApiSecret);
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", credentials.ApiKey);
        }

        public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync("/api/v3/ping", cancellationToken);
            return response.IsSuccessStatusCode;
        }

        public async Task<decimal> GetPriceAsync(
            string symbol, CancellationToken cancellationToken = default)
        {
            var url = $"/api/v3/ticker/price?symbol={Uri.EscapeDataString(symbol.ToUpperInvariant())}";
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var priceStr = doc.RootElement.GetProperty("price").GetString()!;
            return decimal.Parse(priceStr, CultureInfo.InvariantCulture);
        }

        public async Task<OrderResult> PlaceOrderAsync(
            OrderRequest request, CancellationToken cancellationToken = default)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var queryString = BuildOrderParameters(request, timestamp);
            var signature = ComputeSignature(queryString);
            var body = $"{queryString}&signature={signature}";

            using var content = new StringContent(
                body, Encoding.UTF8, "application/x-www-form-urlencoded");
            using var response = await _httpClient.PostAsync(
                "/api/v3/order", content, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new OrderResult(
                    Success: false,
                    OrderId: null,
                    Symbol: request.Symbol,
                    Side: request.Side,
                    ExecutedQuantity: 0,
                    ExecutedPrice: 0,
                    Timestamp: DateTime.UtcNow,
                    ErrorMessage: ParseErrorMessage(json));
            }

            return ParseOrderResponse(json, request);
        }

        #region Request Building

        private static string BuildOrderParameters(OrderRequest request, long timestamp)
        {
            var sb = new StringBuilder();
            sb.Append($"symbol={request.Symbol.ToUpperInvariant()}");
            sb.Append($"&side={MapOrderSide(request.Side)}");
            sb.Append($"&type={MapOrderType(request.Type)}");

            if (request.Type is OrderType.Limit or OrderType.StopLimit)
                sb.Append($"&timeInForce={request.TimeInForce}");

            sb.Append($"&quantity={request.Quantity.ToString(CultureInfo.InvariantCulture)}");

            if (request.Price.HasValue && request.Type is not OrderType.Market)
                sb.Append($"&price={request.Price.Value.ToString(CultureInfo.InvariantCulture)}");

            if (request.StopPrice.HasValue && request.Type is OrderType.Stop or OrderType.StopLimit)
                sb.Append($"&stopPrice={request.StopPrice.Value.ToString(CultureInfo.InvariantCulture)}");

            sb.Append($"&timestamp={timestamp}");
            return sb.ToString();
        }

        private static string MapOrderSide(OrderSide side) => side switch
        {
            OrderSide.Buy => "BUY",
            OrderSide.Sell => "SELL",
            _ => throw new ArgumentOutOfRangeException(nameof(side))
        };

        private static string MapOrderType(OrderType type) => type switch
        {
            OrderType.Market => "MARKET",
            OrderType.Limit => "LIMIT",
            OrderType.Stop => "STOP_LOSS",
            OrderType.StopLimit => "STOP_LOSS_LIMIT",
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        #endregion

        #region Signing

        private string ComputeSignature(string queryString)
        {
            var dataBytes = Encoding.UTF8.GetBytes(queryString);
            var hash = HMACSHA256.HashData(_secretKeyBytes, dataBytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        #endregion

        #region Response Parsing

        private static string ParseErrorMessage(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var code = root.GetProperty("code").GetInt32();
                var msg = root.GetProperty("msg").GetString();
                return $"Binance API Hatası [{code}]: {msg}";
            }
            catch (JsonException)
            {
                return $"API yanıtı ayrıştırılamadı: {json[..Math.Min(json.Length, 200)]}";
            }
        }

        private static OrderResult ParseOrderResponse(string json, OrderRequest request)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var orderId = root.GetProperty("orderId").GetInt64()
                .ToString(CultureInfo.InvariantCulture);
            var executedQty = decimal.Parse(
                root.GetProperty("executedQty").GetString()!, CultureInfo.InvariantCulture);

            var executedPrice = CalculateAveragePrice(root);

            var transactTime = root.GetProperty("transactTime").GetInt64();
            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(transactTime).UtcDateTime;

            return new OrderResult(
                Success: true,
                OrderId: orderId,
                Symbol: request.Symbol,
                Side: request.Side,
                ExecutedQuantity: executedQty,
                ExecutedPrice: executedPrice,
                Timestamp: timestamp);
        }

        private static decimal CalculateAveragePrice(JsonElement root)
        {
            if (root.TryGetProperty("fills", out var fills) && fills.GetArrayLength() > 0)
            {
                decimal totalCost = 0;
                decimal totalQty = 0;
                foreach (var fill in fills.EnumerateArray())
                {
                    var p = decimal.Parse(
                        fill.GetProperty("price").GetString()!, CultureInfo.InvariantCulture);
                    var q = decimal.Parse(
                        fill.GetProperty("qty").GetString()!, CultureInfo.InvariantCulture);
                    totalCost += p * q;
                    totalQty += q;
                }
                if (totalQty > 0)
                    return totalCost / totalQty;
            }

            if (root.TryGetProperty("price", out var priceProp))
            {
                var priceStr = priceProp.GetString()!;
                return decimal.Parse(priceStr, CultureInfo.InvariantCulture);
            }

            return 0;
        }

        #endregion

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
