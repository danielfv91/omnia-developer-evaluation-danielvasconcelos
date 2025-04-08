using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services.Interfaces
{
    public interface ISaleItemBuilder
    {
        List<SaleItem> Build<TItemDto>(IEnumerable<TItemDto> items, Guid saleId)
            where TItemDto : ISaleItemInput;

        decimal CalculateTotalAmount(List<SaleItem> items);
    }

    public interface ISaleItemInput
    {
        Guid ProductId { get; }
        string ProductName { get; }
        int Quantity { get; }
        decimal UnitPrice { get; }
    }
}
