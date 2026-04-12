namespace VortexTrade
{
    public interface IOrderService
    {
        Task<OrderResult> ExecuteOrderAsync(
            OrderRequest request,
            CancellationToken cancellationToken = default);
    }
}
