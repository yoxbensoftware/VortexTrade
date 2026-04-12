namespace VortexTrade
{
    public record DevLogEntry(
        string Version,
        DateTime Date,
        string Developer,
        string MachineName,
        string Description);
}
