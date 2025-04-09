using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Domain.Services
{
    public class SaleItemCalculatorTests
    {
        private readonly ISaleItemCalculator _calculator;

        public SaleItemCalculatorTests()
        {
            _calculator = new SaleItemCalculator();
        }

        [Theory]
        [MemberData(nameof(GetDiscountScenarios))]
        public void CalculateDiscount_Should_Return_Expected_Percentage(int quantity, decimal expected)
        {
            var discount = _calculator.CalculateDiscount(quantity);
            discount.Should().Be(expected);
        }

        public static IEnumerable<object[]> GetDiscountScenarios()
        {
            yield return new object[] { 2, 0 };
            yield return new object[] { 4, 10 };
            yield return new object[] { 10, 20 };
            yield return new object[] { 21, 0 };
        }

        [Theory]
        [MemberData(nameof(GetTotalCalculationScenarios))]
        public void CalculateTotalItem_Should_Calculate_Correct_Total(int quantity, decimal unitPrice, decimal discount, decimal expectedTotal)
        {
            var total = _calculator.CalculateTotalItem(quantity, unitPrice, discount);
            total.Should().BeApproximately(expectedTotal, 0.01m);
        }

        public static IEnumerable<object[]> GetTotalCalculationScenarios()
        {
            var faker = new Faker();

            var quantity1 = 5;
            var price1 = 100m;
            var discount1 = 10m;
            var total1 = price1 * quantity1 * (1 - discount1 / 100);

            var quantity2 = 10;
            var price2 = 50m;
            var discount2 = 20m;
            var total2 = price2 * quantity2 * (1 - discount2 / 100);

            var quantity3 = 3;
            var price3 = 30m;
            var discount3 = 0m;
            var total3 = price3 * quantity3;

            return new List<object[]>
            {
                new object[] { quantity1, price1, discount1, total1 },
                new object[] { quantity2, price2, discount2, total2 },
                new object[] { quantity3, price3, discount3, total3 }
            };
        }
    }
}
