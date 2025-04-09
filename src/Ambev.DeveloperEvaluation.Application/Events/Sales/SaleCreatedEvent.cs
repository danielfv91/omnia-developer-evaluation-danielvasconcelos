using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Application.Events.Sales
{
    public class SaleCreatedEvent : ISaleEvent
    {
        public Guid SaleId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public int SaleNumber { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
    }
}