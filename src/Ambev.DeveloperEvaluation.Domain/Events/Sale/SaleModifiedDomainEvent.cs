namespace Ambev.DeveloperEvaluation.Domain.Events.Sale;

public class SaleModifiedDomainEvent
{
    public Guid SaleId { get; init; }
    public int SaleNumber { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
