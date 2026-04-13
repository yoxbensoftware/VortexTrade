namespace VortexTrade
{
    public sealed record MarketTicker(
        string Exchange,
        string BaseCurrency,
        string QuoteCurrency,
        decimal Price,
        decimal PriceUsd,
        decimal Volume,
        decimal VolumeUsd,
        DateTime LastTraded,
        string Source);
}
