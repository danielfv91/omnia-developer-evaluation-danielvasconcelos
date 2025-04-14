using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Sales.TestData
{

    public static class SaleDomainTestData
    {
        public static IEnumerable<object[]> DiscountScenarios =>
            new List<object[]>
            {
            new object[] { 2, 10m, 0m, 20m },
            new object[] { 5, 10m, 10m, 45m },
            new object[] { 15, 10m, 20m, 120m }
            };

        public static List<SaleItemInput> BuildItemList(int quantity, decimal unitPrice)
        {
            return new List<SaleItemInput>
        {
            new()
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                Quantity = quantity,
                UnitPrice = unitPrice
            }
        };
        }

        public static List<SaleItemInput> BuildItemListWithProducts(params Guid[] productIds)
        {
            var faker = new Faker();
            return productIds.Select(id => new SaleItemInput
            {
                ProductId = id,
                ProductName = faker.Commerce.ProductName(),
                Quantity = 2,
                UnitPrice = 10
            }).ToList();
        }

        public static Sale CreateDefaultSale()
        {
            return Sale.Create(
                saleNumber: 123,
                saleDate: DateTime.UtcNow,
                customerId: Guid.NewGuid(),
                customerName: "Customer Test",
                branch: "Branch A",
                items: BuildItemList(3, 10m)
            );
        }

        public static (Sale Original, List<SaleItemInput> UpdatedItems) CreateSaleWithRemovedItem()
        {
            var product1 = Guid.NewGuid();
            var product2 = Guid.NewGuid();

            var sale = Sale.Create(
                saleNumber: 123,
                saleDate: DateTime.UtcNow,
                customerId: Guid.NewGuid(),
                customerName: "Customer Test",
                branch: "Branch X",
                items: BuildItemListWithProducts(product1, product2)
            );

            var updatedItems = BuildItemListWithProducts(product1);

            return (sale, updatedItems);
        }

        public static Sale CreateCancelledSale()
        {
            var sale = CreateDefaultSale();
            sale.Cancel("Test reason");
            return sale;
        }
    }
}