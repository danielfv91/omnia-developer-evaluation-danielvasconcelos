using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Sales.TestData
{

    public static class SalesValidatorTestData
    {
        public static CreateSaleCommand CreateValidCreateCommand()
        {
            return new CreateSaleCommand
            {
                SaleNumber = 123,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Valid Customer",
                Branch = "Valid Branch",
                Items = new List<CreateSaleItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product A",
                    Quantity = 2,
                    UnitPrice = 10
                }
            }
            };
        }

        public static CreateSaleCommand CreateInvalidCreateCommand()
        {
            return new CreateSaleCommand
            {
                SaleNumber = 0,
                CustomerId = Guid.Empty,
                CustomerName = "",
                Branch = "",
                Items = new List<CreateSaleItemDto>
            {
                new()
                {
                    ProductId = Guid.Empty,
                    ProductName = "",
                    Quantity = 0,
                    UnitPrice = -5
                }
            }
            };
        }

        public static UpdateSaleCommand CreateValidUpdateCommand()
        {
            return new UpdateSaleCommand
            {
                Id = Guid.NewGuid(),
                SaleNumber = 123,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Valid Customer",
                Branch = "Valid Branch",
                Items = new List<UpdateSaleItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product A",
                    Quantity = 2,
                    UnitPrice = 10
                }
            }
            };
        }

        public static UpdateSaleCommand CreateInvalidUpdateCommand()
        {
            return new UpdateSaleCommand
            {
                Id = Guid.Empty,
                SaleNumber = 0,
                SaleDate = default,
                CustomerId = Guid.Empty,
                CustomerName = "",
                Branch = "",
                Items = new List<UpdateSaleItemDto>
            {
                new()
                {
                    ProductId = Guid.Empty,
                    ProductName = "",
                    Quantity = 25,
                    UnitPrice = 0
                }
            }
            };
        }

        public static GetSalesQuery GenerateValidQuery()
        {
            return new Faker<GetSalesQuery>()
                .RuleFor(q => q.Page, f => f.Random.Int(1, 5))
                .RuleFor(q => q.Size, f => f.Random.Int(1, 100))
                .RuleFor(q => q.Order, f => f.PickRandom(new[]
                {
                    "SaleDate desc", "Branch asc", "CustomerName desc"
                }))
                .RuleFor(q => q.MinDate, f => f.Date.Past(1))
                .RuleFor(q => q.MaxDate, (f, q) => q.MinDate?.AddDays(f.Random.Int(1, 10)));
        }

        public static GetSalesQuery GenerateQueryWithInvalidOrder()
        {
            var query = GenerateValidQuery();
            query.Order = "InvalidField asc";
            return query;
        }
        public static GetSalesQuery GenerateQueryWithFutureDates()
        {
            var futureDate = DateTime.UtcNow.AddDays(10);
            var query = GenerateValidQuery();
            query.MinDate = futureDate;
            query.MaxDate = futureDate.AddDays(2);
            return query;
        }

    }
}