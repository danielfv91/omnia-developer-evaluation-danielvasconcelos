namespace Ambev.DeveloperEvaluation.Domain.Events.Sale;

public class SaleCreatedDomainEvent
{
    public Guid SaleId { get; init; }
    public int SaleNumber { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
