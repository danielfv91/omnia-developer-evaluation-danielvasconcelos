using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validators
{

    public class UpdateSaleValidatorTests
    {
        private readonly UpdateSaleValidator _validator = new();

        [Fact]
        public void Should_Have_Errors_When_Command_Is_Invalid()
        {
            // Arrange
            var invalidCommand = SalesValidatorTestData.CreateInvalidUpdateCommand();

            // Act & Assert
            var result = _validator.TestValidate(invalidCommand);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.Id);
            result.ShouldHaveValidationErrorFor(c => c.SaleNumber);
            result.ShouldHaveValidationErrorFor(c => c.SaleDate);
            result.ShouldHaveValidationErrorFor(c => c.CustomerId);
            result.ShouldHaveValidationErrorFor(c => c.CustomerName);
            result.ShouldHaveValidationErrorFor(c => c.Branch);
            result.ShouldHaveValidationErrorFor("Items[0].ProductId");
            result.ShouldHaveValidationErrorFor("Items[0].ProductName");
            result.ShouldHaveValidationErrorFor("Items[0].Quantity");
            result.ShouldHaveValidationErrorFor("Items[0].UnitPrice");

            result.Errors.Should().Contain(e => e.ErrorMessage == "Cannot sell more than 20 identical items per product.");
        }


        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Valid()
        {
            // Arrange
            var validCommand = SalesValidatorTestData.CreateValidUpdateCommand();

            // Act & Assert
            var result = _validator.TestValidate(validCommand);

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}