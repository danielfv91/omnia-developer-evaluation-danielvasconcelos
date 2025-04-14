using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.Builders
{

    public static class SaleCommandFakerBuilder
    {
        public static CreateSaleCommand BuildValidSale(int quantity = 5, decimal unitPrice = 10m)
        {
            var faker = new Faker();

            return new CreateSaleCommand
            {
                SaleNumber = faker.Random.Int(1000, 9999),
                SaleDate = faker.Date.Recent().ToUniversalTime(),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Person.FullName,
                Branch = faker.Company.CompanyName(),
                Items = new List<CreateSaleItemDto>
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

        public static CreateSaleCommand BuildSaleWithInvalidQuantity(int quantity)
        {
            var sale = BuildValidSale();
            sale.Items[0].Quantity = quantity;
            return sale;
        }

        public static CreateSaleCommand BuildSaleWithMultipleItems(params (int quantity, decimal unitPrice)[] items)
        {
            var faker = new Faker();

            return new CreateSaleCommand
            {
                SaleNumber = faker.Random.Int(1000, 9999),
                SaleDate = faker.Date.Recent().ToUniversalTime(),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Person.FullName,
                Branch = faker.Company.CompanyName(),
                Items = items.Select(i => new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = i.quantity,
                    UnitPrice = i.unitPrice
                }).ToList()
            };
        }
    }
}