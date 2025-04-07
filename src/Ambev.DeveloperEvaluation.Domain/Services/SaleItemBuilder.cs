using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;

namespace Ambev.DeveloperEvaluation.Domain.Services
{
    public class SaleItemBuilder : ISaleItemBuilder
    {
        private readonly ISaleItemCalculator _calculator;

        public SaleItemBuilder(ISaleItemCalculator calculator)
        {
            _calculator = calculator;
        }

        public List<SaleItem> Build<TItemDto>(IEnumerable<TItemDto> items, Guid saleId)
            where TItemDto : ISaleItemInput
        {
            var result = new List<SaleItem>();

            foreach (var item in items)
            {
                var discount = _calculator.CalculateDiscount(item.Quantity);
                var total = _calculator.CalculateTotalItem(item.Quantity, item.UnitPrice, discount);

                result.Add(new SaleItem
                {
                    Id = Guid.NewGuid(),
                    SaleId = saleId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = discount,
                    TotalItemAmount = total,
                    IsCancelled = false
                });
            }

            return result;
        }

        public decimal CalculateTotalAmount(List<SaleItem> items)
        {
            return items.Sum(i => i.TotalItemAmount);
        }
    }
}
