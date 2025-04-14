namespace Ambev.DeveloperEvaluation.Domain.Events.Sale;

public class SaleCancelledDomainEvent
{
    public Guid SaleId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
