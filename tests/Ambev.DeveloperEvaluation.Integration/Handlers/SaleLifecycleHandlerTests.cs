using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Integration.Builders;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers
{

    public class SaleLifecycleHandlerTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IServiceProvider _provider;

        public SaleLifecycleHandlerTests(IntegrationTestFixture fixture)
        {
            _provider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task SaleLifecycle_Should_CreateReadUpdateDeleteSuccessfully()
        {
            var mediator = _provider.GetRequiredService<IMediator>();
            var context = _provider.GetRequiredService<DefaultContext>();

            // 1. Create
            var createCommand = SaleCommandFakerBuilder.BuildValidSale(quantity: 5, unitPrice: 10m);
            var createResult = await mediator.Send(createCommand);

            createResult.TotalAmount.Should().Be(45);

            // 2. Get (after creation)
            var getQuery = new GetSaleQuery { Id = createResult.Id };
            var getResult = await mediator.Send(getQuery);

            getResult.Should().NotBeNull();
            getResult!.SaleNumber.Should().Be(createCommand.SaleNumber);

            // 3. Update
            var updateCommand = new UpdateSaleCommand
            {
                Id = createResult.Id,
                SaleNumber = createResult.SaleNumber,
                SaleDate = getResult.SaleDate,
                CustomerId = createCommand.CustomerId,
                CustomerName = "Updated Customer",
                Branch = "Updated Branch",
                Items = new List<UpdateSaleItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "New Product",
                    Quantity = 10,
                    UnitPrice = 10
                }
            }
            };

            var updateResult = await mediator.Send(updateCommand);
            updateResult.TotalAmount.Should().Be(80);

            // 4. Delete
            var deleteCommand = new DeleteSaleCommand(createResult.Id);
            var deleteResult = await mediator.Send(deleteCommand);
            deleteResult.Should().BeTrue();

            // 5. Get again → should throw NotFoundException
            Func<Task> act = async () => await mediator.Send(new GetSaleQuery { Id = createResult.Id });
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Sale with ID {createResult.Id} was not found.");
        }
    }
}