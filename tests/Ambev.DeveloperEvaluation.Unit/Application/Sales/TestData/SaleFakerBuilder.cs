using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Sales.TestData
{
    public static class SaleFakerBuilder
    {
        public static CreateSaleCommand CreateValidCreateCommand(int items = 1)
        {
            var faker = new Faker();
            return new CreateSaleCommand
            {
                SaleNumber = faker.Random.Int(1000, 9999),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Name.FullName(),
                Branch = faker.Company.CompanyName(),
                SaleDate = faker.Date.Recent(),
                Items = Enumerable.Range(1, items).Select(_ => new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = faker.Random.Int(1, 10),
                    UnitPrice = faker.Random.Decimal(10, 100)
                }).ToList()
            };
        }

        public static UpdateSaleCommand CreateValidUpdateCommand(Guid saleId, int items = 1)
        {
            var faker = new Faker();
            return new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = faker.Random.Int(1000, 9999),
                SaleDate = faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Name.FullName(),
                Branch = faker.Company.CompanyName(),
                Items = Enumerable.Range(1, items).Select(_ => new UpdateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = faker.Random.Int(1, 10),
                    UnitPrice = faker.Random.Decimal(10, 100)
                }).ToList()
            };
        }

        public static Sale GenerateValidSale()
        {
            var faker = new Faker();
            return new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = faker.Random.Int(1000, 9999),
                SaleDate = faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Name.FullName(),
                Branch = faker.Company.CompanyName(),
                Items = new List<SaleItem>(),
                TotalAmount = 0
            };
        }

        public static Sale GenerateValidSaleWithItems(int items = 1)
        {
            var faker = new Faker();
            var saleId = Guid.NewGuid();

            return new Sale
            {
                Id = saleId,
                SaleNumber = faker.Random.Int(1000, 9999),
                SaleDate = faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Name.FullName(),
                Branch = faker.Company.CompanyName(),
                Items = Enumerable.Range(1, items).Select(_ => new SaleItem
                {
                    Id = Guid.NewGuid(),
                    SaleId = saleId,
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = faker.Random.Int(1, 5),
                    UnitPrice = faker.Random.Decimal(10, 50),
                    DiscountPercentage = 0,
                    TotalItemAmount = faker.Random.Decimal(10, 200),
                    IsCancelled = false
                }).ToList(),
                TotalAmount = faker.Random.Decimal(100, 500)
            };
        }
    }
}
