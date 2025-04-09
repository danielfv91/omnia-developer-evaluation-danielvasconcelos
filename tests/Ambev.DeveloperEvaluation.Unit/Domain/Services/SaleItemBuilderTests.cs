using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services
{
    public class SaleItemBuilderTests
    {
        private readonly ISaleItemCalculator _calculator = Substitute.For<ISaleItemCalculator>();
        private readonly ISaleItemBuilder _builder;

        public SaleItemBuilderTests()
        {
            _builder = new SaleItemBuilder(_calculator);
        }

        private class FakeItem : ISaleItemInput
        {
            public Guid ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        [Fact]
        public void Build_Should_CreateSaleItems_Correctly()
        {
            // Arrange
            var items = new List<FakeItem>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Teste",
                    Quantity = 5,
                    UnitPrice = 10
                }
            };

            _calculator.CalculateDiscount(5).Returns(10);
            _calculator.CalculateTotalItem(5, 10, 10).Returns(45); // 50 - 10%

            var saleId = Guid.NewGuid();

            // Act
            var result = _builder.Build(items, saleId);

            // Assert
            result.Should().HaveCount(1);
            result[0].ProductId.Should().Be(items[0].ProductId);
            result[0].ProductName.Should().Be(items[0].ProductName);
            result[0].Quantity.Should().Be(5);
            result[0].UnitPrice.Should().Be(10);
            result[0].DiscountPercentage.Should().Be(10);
            result[0].TotalItemAmount.Should().Be(45);
            result[0].SaleId.Should().Be(saleId);
        }

        [Fact]
        public void CalculateTotalAmount_Should_Sum_Correctly()
        {
            // Arrange
            var items = new List<SaleItem>
            {
                new() { TotalItemAmount = 50 },
                new() { TotalItemAmount = 30.5m }
            };

            // Act
            var total = _builder.CalculateTotalAmount(items);

            // Assert
            total.Should().Be(80.5m);
        }

    }
}
