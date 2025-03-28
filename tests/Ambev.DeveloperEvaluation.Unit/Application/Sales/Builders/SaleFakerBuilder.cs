using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Builders
{
    public static class SaleFakerBuilder
    {
        private static readonly Faker Faker = new("en");

        public static CreateSaleCommand CreateValidCreateCommand(int items = 1)
        {
            return new CreateSaleCommand
            {
                SaleNumber = Faker.Random.Int(1000, 9999),
                SaleDate = Faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = Faker.Name.FullName(),
                Branch = Faker.Company.CompanyName(),
                Items = GenerateCreateSaleItems(items)
            };
        }

        public static CreateSaleCommand CreateCommandWithInvalidQuantity()
        {
            return new CreateSaleCommand
            {
                SaleNumber = Faker.Random.Int(1000, 9999),
                SaleDate = Faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = Faker.Name.FullName(),
                Branch = Faker.Company.CompanyName(),
                Items = new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = Faker.Commerce.ProductName(),
                        Quantity = 0,
                        UnitPrice = Faker.Random.Decimal(10, 100)
                    }
                }
            };
        }

        public static UpdateSaleCommand CreateValidUpdateCommand(Guid saleId, int items = 1)
        {
            return new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = Faker.Random.Int(1000, 9999),
                SaleDate = Faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = Faker.Name.FullName(),
                Branch = Faker.Company.CompanyName(),
                Items = GenerateUpdateSaleItems(items)
            };
        }

        public static Sale BuildFromCommand(CreateSaleCommand command)
        {
            var items = command.Items.Select(item =>
            {
                var discount = GetDiscountPercentage(item.Quantity);
                return new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = discount,
                    TotalItemAmount = item.Quantity * item.UnitPrice * (1 - discount),
                    IsCancelled = false
                };
            }).ToList();

            return new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = command.SaleNumber,
                SaleDate = command.SaleDate,
                CustomerId = command.CustomerId,
                CustomerName = command.CustomerName,
                Branch = command.Branch,
                Items = items,
                TotalAmount = items.Sum(i => i.TotalItemAmount),
                IsCancelled = false
            };
        }

        public static Sale GenerateValidSale(int items = 2)
        {
            var saleItems = new List<SaleItem>();
            for (int i = 0; i < items; i++)
            {
                var quantity = Faker.Random.Int(1, 20);
                var unitPrice = Faker.Random.Decimal(10, 100);
                var discount = GetDiscountPercentage(quantity);

                saleItems.Add(new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = Faker.Commerce.ProductName(),
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountPercentage = discount,
                    TotalItemAmount = quantity * unitPrice * (1 - discount),
                    IsCancelled = false
                });
            }

            return new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = Faker.Random.Int(1000, 9999),
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = Faker.Name.FullName(),
                Branch = Faker.Company.CompanyName(),
                Items = saleItems,
                TotalAmount = saleItems.Sum(i => i.TotalItemAmount),
                IsCancelled = false
            };
        }

        private static List<CreateSaleItemDto> GenerateCreateSaleItems(int count)
        {
            var list = new List<CreateSaleItemDto>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = Faker.Commerce.ProductName(),
                    Quantity = Faker.Random.Int(1, 20),
                    UnitPrice = Faker.Random.Decimal(10, 100)
                });
            }

            return list;
        }

        private static List<UpdateSaleItemDto> GenerateUpdateSaleItems(int count)
        {
            var list = new List<UpdateSaleItemDto>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new UpdateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = Faker.Commerce.ProductName(),
                    Quantity = Faker.Random.Int(1, 20),
                    UnitPrice = Faker.Random.Decimal(10, 100)
                });
            }

            return list;
        }
        public static Sale GenerateValidSaleWithItems(int items = 1)
        {
            var saleId = Guid.NewGuid();

            return new Sale
            {
                Id = saleId,
                SaleNumber = Faker.Random.Int(1000, 9999),
                SaleDate = Faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = Faker.Name.FullName(),
                Branch = Faker.Company.CompanyName(),
                IsCancelled = false,
                TotalAmount = 0,
                Items = Enumerable.Range(0, items).Select(_ => new SaleItem
                {
                    Id = Guid.NewGuid(),
                    SaleId = saleId,
                    ProductId = Guid.NewGuid(),
                    ProductName = Faker.Commerce.ProductName(),
                    Quantity = Faker.Random.Int(1, 10),
                    UnitPrice = Faker.Random.Decimal(10, 100),
                    DiscountPercentage = 0,
                    TotalItemAmount = 0,
                    IsCancelled = false
                }).ToList()
            };
        }

        private static decimal GetDiscountPercentage(int quantity)
        {
            if (quantity >= 10) return 0.20m;
            if (quantity >= 4) return 0.10m;
            return 0.00m;
        }
    }
}
