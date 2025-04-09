namespace Ambev.DeveloperEvaluation.Application.Events.Interfaces;

public interface ISaleEvent
{
    Guid SaleId { get; }
    DateTime Timestamp { get; }
}
