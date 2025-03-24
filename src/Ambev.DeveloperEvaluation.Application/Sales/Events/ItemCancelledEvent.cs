namespace Ambev.DeveloperEvaluation.Application.Sales.Events
{
    public class ItemCancelledEvent : ISaleEvent
    {
        public Guid SaleId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
    }
}