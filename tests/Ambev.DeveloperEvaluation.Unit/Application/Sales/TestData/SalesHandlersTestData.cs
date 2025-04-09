using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Sales.TestData
{
    public static class SalesHandlersTestData
    {

        public static IEnumerable<object[]> GenerateWithBogus()
        {
            return GenerateWithBogus(3);
        }

        public static IEnumerable<object[]> DiscountScenarios =>
            new List<object[]>
            {
                new object[] { 2, 10m, 0m, 20m },
                new object[] { 5, 10m, 10m, 45m },
                new object[] { 15, 10m, 20m, 120m }
            };

        public static IEnumerable<object[]> GenerateWithBogus(int count = 3)
        {
            var faker = new Faker();

            for (int i = 0; i < count; i++)
            {
                var quantity = faker.Random.Int(1, 25);
                var unitPrice = faker.Random.Decimal(5m, 50m);
                var discount = GetExpectedDiscount(quantity);
                var total = quantity * unitPrice * (1 - discount / 100);

                yield return new object[] { quantity, unitPrice, discount, Math.Round(total, 2) };
            }
        }

        public static CreateSaleCommand CreateCommand(int quantity, decimal unitPrice)
        {
            var faker = new Faker();

            return new CreateSaleCommand
            {
                SaleNumber = faker.Random.Int(1000, 9999),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Name.FullName(),
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

        private static decimal GetExpectedDiscount(int quantity)
        {
            if (quantity >= 10 && quantity <= 20) return 20;
            if (quantity >= 4 && quantity < 10) return 10;
            return 0;
        }
    }
}
