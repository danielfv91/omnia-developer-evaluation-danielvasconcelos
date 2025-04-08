using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validators
{
    public class GetSalesValidatorTests
    {
        private readonly GetSalesValidator _validator = new();

        [Fact]
        public void Validate_Should_Pass_When_AllFieldsAreValid()
        {
            var query = SalesValidatorTestData.GenerateValidQuery();

            var result = _validator.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(-1, 10)]
        [InlineData(1, 0)]
        [InlineData(1, 101)]
        public void Validate_Should_Fail_When_PaginationIsInvalid(int page, int size)
        {
            var query = SalesValidatorTestData.GenerateValidQuery();
            query.Page = page;
            query.Size = size;

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_Should_Fail_When_OrderFieldIsInvalid()
        {
            var query = SalesValidatorTestData.GenerateQueryWithInvalidOrder();

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Order");
        }

        [Fact]
        public void Validate_Should_Fail_When_DatesAreInFuture()
        {
            var query = SalesValidatorTestData.GenerateQueryWithFutureDates();

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().OnlyContain(e =>
                e.PropertyName == "MinDate" || e.PropertyName == "MaxDate");
        }
    }
}
