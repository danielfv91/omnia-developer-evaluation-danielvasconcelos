using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validators
{
    public class CreateSaleValidatorTests
    {
        private readonly CreateSaleValidator _validator = new();

        [Fact]
        public void Should_Have_Errors_When_Command_Is_Invalid()
        {
            // Arrange
            var command = SalesValidatorTestData.CreateInvalidCreateCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.SaleNumber);
            result.ShouldHaveValidationErrorFor(c => c.CustomerId);
            result.ShouldHaveValidationErrorFor(c => c.CustomerName);
            result.ShouldHaveValidationErrorFor(c => c.Branch);
            result.ShouldHaveValidationErrorFor("Items[0].ProductId");
            result.ShouldHaveValidationErrorFor("Items[0].ProductName");
            result.ShouldHaveValidationErrorFor("Items[0].Quantity");
            result.ShouldHaveValidationErrorFor("Items[0].UnitPrice");

        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Valid()
        {
            // Arrange
            var command = SalesValidatorTestData.CreateValidCreateCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}