namespace Ambev.DeveloperEvaluation.Application.Sales.Events
{
    public interface ISaleEvent
    {
        Guid SaleId { get; }
        DateTime Timestamp { get; }
    }
}