using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.Builders;

public static class SaleUpdateFakerBuilder
{
    public static Sale CreateValidEntity(int quantity = 5, decimal unitPrice = 10m)
    {
        var faker = new Faker();

        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            SaleId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ProductName = faker.Commerce.ProductName(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            DiscountPercentage = CalculateDiscount(quantity),
            TotalItemAmount = CalculateTotal(quantity, unitPrice),
            IsCancelled = false
        };

        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = faker.Random.Int(1000, 9999),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Person.FullName,
            Branch = faker.Company.CompanyName(),
            Items = new List<SaleItem> { item },
            TotalAmount = item.TotalItemAmount
        };
    }

    public static UpdateSaleCommand CreateValidUpdateCommand(Guid saleId, int quantity = 5, decimal unitPrice = 10m)
    {
        var faker = new Faker();

        return new UpdateSaleCommand
        {
            Id = saleId,
            SaleNumber = faker.Random.Int(1000, 9999),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Person.FullName,
            Branch = faker.Company.CompanyName(),
            Items = new List<UpdateSaleItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            }
        };
    }

    private static decimal CalculateDiscount(int quantity)
    {
        if (quantity >= 10 && quantity <= 20) return 20;
        if (quantity >= 4 && quantity < 10) return 10;
        return 0;
    }

    private static decimal CalculateTotal(int quantity, decimal unitPrice)
    {
        var discount = CalculateDiscount(quantity);
        var total = quantity * unitPrice;
        return total - (total * discount / 100);
    }
}
