namespace VortexTrade
{
    public sealed record TickerInfo(
        string Exchange,
        string Base,
        string Target,
        decimal LastPrice,
        decimal UsdPrice,
        decimal Volume,
        decimal UsdVolume,
        decimal SpreadPercent,
        string TradeUrl,
        DateTime LastTraded);
}
