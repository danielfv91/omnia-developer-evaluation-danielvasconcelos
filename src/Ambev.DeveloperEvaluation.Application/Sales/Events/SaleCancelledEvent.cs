namespace Ambev.DeveloperEvaluation.Application.Sales.Events
{
    public class SaleCancelledEvent : ISaleEvent
    {
        public Guid SaleId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string Reason { get; init; } = string.Empty;
    }
}