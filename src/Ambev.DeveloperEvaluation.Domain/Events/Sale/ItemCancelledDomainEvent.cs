namespace Ambev.DeveloperEvaluation.Domain.Events.Sale;

public class ItemCancelledDomainEvent
{
    public Guid SaleId { get; init; }
    public Guid ItemId { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Reason { get; init; } = "Item removed during sale update";
    public DateTime CancelledAt { get; init; } = DateTime.UtcNow;
}
