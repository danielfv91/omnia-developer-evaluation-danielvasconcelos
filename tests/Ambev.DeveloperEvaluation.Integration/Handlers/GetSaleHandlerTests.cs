using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers
{

    public class GetSaleHandlerTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IServiceProvider _provider;

        public GetSaleHandlerTests(IntegrationTestFixture fixture)
        {
            _provider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task GetSale_Should_ReturnSale_WhenExists()
        {
            // Arrange
            var context = _provider.GetRequiredService<DefaultContext>();
            var mediator = _provider.GetRequiredService<IMediator>();

            var sale = Sale.Create(
                saleNumber: 123,
                saleDate: DateTime.UtcNow,
                customerId: Guid.NewGuid(),
                customerName: "Daniel",
                branch: "SP",
                items: new List<SaleItemInput>()
            );

            await context.Sales.AddAsync(sale);
            await context.SaveChangesAsync();

            var query = new GetSaleQuery { Id = sale.Id };

            // Act
            var result = await mediator.Send(query);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(sale.Id);
            result.SaleNumber.Should().Be(sale.SaleNumber);
        }

        [Fact]
        public async Task GetSale_Should_ThrowNotFoundException_WhenSaleDoesNotExist()
        {
            var mediator = _provider.GetRequiredService<IMediator>();

            var act = async () => await mediator.Send(new GetSaleQuery { Id = Guid.NewGuid() });

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*not found*");
        }


        [Fact]
        public async Task GetSale_Should_ThrowValidationException_WhenIdIsEmpty()
        {
            var mediator = _provider.GetRequiredService<IMediator>();
            var query = new GetSaleQuery { Id = Guid.Empty };

            Func<Task> act = async () => await mediator.Send(query);

            await act.Should().ThrowAsync<ValidationException>()
                .Where(ex => ex.Errors.Any(e => e.ErrorMessage.Contains("Sale ID must be provided")));
        }
    }
}