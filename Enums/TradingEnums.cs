namespace VortexTrade
{
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting,
        Error
    }

    public enum OrderSide
    {
        Buy,
        Sell
    }

    public enum OrderType
    {
        Market,
        Limit,
        Stop,
        StopLimit
    }

    public enum OrderStatus
    {
        Pending,
        Open,
        PartiallyFilled,
        Filled,
        Cancelled,
        Rejected
    }

    public enum TimeFrame
    {
        M1,
        M5,
        M15,
        M30,
        H1,
        H4,
        D1,
        W1,
        MN1
    }

    public enum ExchangeType
    {
        Binance
    }

    public enum TimeInForce
    {
        GTC,
        IOC,
        FOK
    }

    public enum ScheduledOrderStatus
    {
        Pending,
        Executing,
        Completed,
        Failed,
        Cancelled
    }
}
