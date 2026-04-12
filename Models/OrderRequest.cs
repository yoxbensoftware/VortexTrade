namespace VortexTrade
{
    public sealed record OrderRequest(
        string Symbol,
        OrderSide Side,
        OrderType Type,
        decimal Quantity,
        decimal? Price = null,
        decimal? StopPrice = null,
        TimeInForce TimeInForce = TimeInForce.GTC);
}
