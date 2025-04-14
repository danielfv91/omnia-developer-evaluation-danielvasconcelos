using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.Builders
{

    public static class SaleUpdateFakerBuilder
    {
        public static Sale CreateValidEntity(int quantity = 5, decimal unitPrice = 10m)
        {
            var faker = new Faker();

            var itemInput = new SaleItemInput
            {
                ProductId = Guid.NewGuid(),
                ProductName = faker.Commerce.ProductName(),
                Quantity = quantity,
                UnitPrice = unitPrice
            };

            var sale = Sale.Create(
                saleNumber: faker.Random.Int(1000, 9999),
                saleDate: DateTime.UtcNow,
                customerId: Guid.NewGuid(),
                customerName: faker.Person.FullName,
                branch: faker.Company.CompanyName(),
                items: new List<SaleItemInput> { itemInput }
            );

            return sale;
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
    }
}