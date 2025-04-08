using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validators
{
    public class GetSaleValidatorTests
    {
        private readonly GetSaleValidator _validator = new();

        [Fact]
        public void Validate_Should_Pass_When_Id_IsValid()
        {
            // Arrange
            var query = new GetSaleQuery { Id = Guid.NewGuid() };

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Id_IsEmpty()
        {
            // Arrange
            var query = new GetSaleQuery { Id = Guid.Empty };

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Id");
        }
    }
}
