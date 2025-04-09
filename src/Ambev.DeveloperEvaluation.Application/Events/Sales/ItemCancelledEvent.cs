using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Application.Events.Sales
{
    public class ItemCancelledEvent : ISaleEvent
    {
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public DateTime CancelledAt { get; set; }
        public string Reason { get; set; } = default!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
