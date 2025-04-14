using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Sales.TestData
{

    public static class SaleFakerBuilder
    {
        private static readonly Faker Faker = new();

        public static CreateSaleCommand CreateValidCreateCommand(int items = 1)
        {
            return new CreateSaleCommand
            {
                SaleNumber = Faker.Random.Int(1000, 9999),
                CustomerId = Guid.NewGuid(),
                CustomerName = Faker.Name.FullName(),
                Branch = Faker.Company.CompanyName(),
                SaleDate = Faker.Date.Recent(),
                Items = GenerateCreateSaleItems(items)
            };
        }

        public static UpdateSaleCommand CreateValidUpdateCommand(Guid saleId, int quantity, decimal unitPrice)
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

        public static Sale GenerateValidSale()
        {
            return Sale.Create(
                saleNumber: Faker.Random.Int(1000, 9999),
                saleDate: Faker.Date.Recent(),
                customerId: Guid.NewGuid(),
                customerName: Faker.Name.FullName(),
                branch: Faker.Company.CompanyName(),
                items: GenerateSaleItemInputs(1)
            );
        }

        public static Sale GenerateValidSaleWithItems(int items = 1)
        {
            return Sale.Create(
                saleNumber: Faker.Random.Int(1000, 9999),
                saleDate: Faker.Date.Recent(),
                customerId: Guid.NewGuid(),
                customerName: Faker.Name.FullName(),
                branch: Faker.Company.CompanyName(),
                items: GenerateSaleItemInputs(items)
            );
        }

        private static List<CreateSaleItemDto> GenerateCreateSaleItems(int count)
        {
            return Enumerable.Range(0, count)
                .Select(_ => new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = Faker.Commerce.ProductName(),
                    Quantity = Faker.Random.Int(1, 10),
                    UnitPrice = Faker.Random.Decimal(10, 100)
                })
                .ToList();
        }

        private static IEnumerable<SaleItemInput> GenerateSaleItemInputs(int count)
        {
            return Enumerable.Range(0, count)
                .Select(_ => new SaleItemInput
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = Faker.Commerce.ProductName(),
                    Quantity = Faker.Random.Int(1, 10),
                    UnitPrice = Faker.Random.Decimal(10, 100)
                });
        }
    }
}