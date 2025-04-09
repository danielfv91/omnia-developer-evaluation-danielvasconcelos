namespace Ambev.DeveloperEvaluation.Domain.Services.Interfaces
{
    public interface ISaleItemCalculator
    {
        decimal CalculateDiscount(int quantity);
        decimal CalculateTotalItem(int quantity, decimal unitPrice, decimal discount);
    }
}
