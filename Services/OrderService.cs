namespace VortexTrade
{
    public sealed class OrderService(IExchange exchange, IOrderValidator validator) : IOrderService
    {
        public async Task<OrderResult> ExecuteOrderAsync(
            OrderRequest request, CancellationToken cancellationToken = default)
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
            {
                return new OrderResult(
                    Success: false,
                    OrderId: null,
                    Symbol: request.Symbol,
                    Side: request.Side,
                    ExecutedQuantity: 0,
                    ExecutedPrice: 0,
                    Timestamp: DateTime.UtcNow,
                    ErrorMessage: string.Join(" | ", validation.Errors));
            }

            return await exchange.PlaceOrderAsync(request, cancellationToken);
        }
    }
}
