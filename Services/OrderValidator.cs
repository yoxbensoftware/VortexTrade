namespace VortexTrade
{
    public sealed class OrderValidator : IOrderValidator
    {
        public OrderValidationResult Validate(OrderRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Symbol))
                errors.Add("Sembol boş olamaz.");

            if (request.Quantity <= 0)
                errors.Add("Miktar sıfırdan büyük olmalıdır.");

            if (request.Type is OrderType.Limit or OrderType.StopLimit)
            {
                if (!request.Price.HasValue || request.Price.Value <= 0)
                    errors.Add("Limit emirleri için fiyat belirtilmelidir.");
            }

            if (request.Type is OrderType.Stop or OrderType.StopLimit)
            {
                if (!request.StopPrice.HasValue || request.StopPrice.Value <= 0)
                    errors.Add("Stop emirleri için stop fiyatı belirtilmelidir.");
            }

            return errors.Count == 0
                ? OrderValidationResult.Valid()
                : OrderValidationResult.WithErrors(errors);
        }
    }
}
