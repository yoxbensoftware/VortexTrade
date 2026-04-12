namespace VortexTrade
{
    public interface IOrderValidator
    {
        OrderValidationResult Validate(OrderRequest request);
    }
}
