namespace VortexTrade
{
    public sealed record ExchangeCredentials(
        string ApiKey,
        string ApiSecret,
        ExchangeType Exchange);
}
