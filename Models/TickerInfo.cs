namespace VortexTrade
{
    public sealed record TickerInfo(
        string Symbol,
        decimal LastPrice,
        decimal PriceChangePercent,
        decimal HighPrice,
        decimal LowPrice,
        decimal Volume,
        decimal QuoteVolume);
}
