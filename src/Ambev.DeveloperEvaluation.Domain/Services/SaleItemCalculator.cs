using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;

namespace Ambev.DeveloperEvaluation.Domain.Services
{
    public class SaleItemCalculator : ISaleItemCalculator
    {
        public decimal CalculateDiscount(int quantity)
        {
            if (quantity >= 10 && quantity <= 20) return 20;
            if (quantity >= 4 && quantity < 10) return 10;
            return 0;
        }

        public decimal CalculateTotalItem(int quantity, decimal unitPrice, decimal discount)
        {
            var total = quantity * unitPrice;
            return total - (total * discount / 100);
        }
    }
}
