using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Application.Events.Sales
{
    public class SaleModifiedEvent : ISaleEvent
    {
        public Guid SaleId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public int SaleNumber { get; init; }
        public decimal TotalAmount { get; init; }
    }
}