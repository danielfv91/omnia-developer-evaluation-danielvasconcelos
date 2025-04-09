using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Application.Events.Sales
{
    public class SaleCancelledEvent : ISaleEvent
    {
        public Guid SaleId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string Reason { get; init; } = string.Empty;
    }
}