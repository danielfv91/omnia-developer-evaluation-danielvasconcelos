using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.Integration.Mocks;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers
{

    public class DeleteSaleHandlerTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IServiceProvider _provider;

        public DeleteSaleHandlerTests(IntegrationTestFixture fixture)
        {
            _provider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task DeleteSale_Should_DeleteExistingSale_AndRaiseDomainEvent()
        {
            var context = _provider.GetRequiredService<DefaultContext>();
            var mediator = _provider.GetRequiredService<IMediator>();
            var eventPublisher = _provider.GetRequiredService<IEventPublisher>() as IEventPublisherMock;
            eventPublisher!.Reset();

            var sale = Sale.Create(
                saleNumber: 1234,
                saleDate: DateTime.UtcNow,
                customerId: Guid.NewGuid(),
                customerName: "Customer A",
                branch: "Branch A",
                items: new List<SaleItemInput>()
            );

            await context.Sales.AddAsync(sale);
            await context.SaveChangesAsync();

            var command = new DeleteSaleCommand(sale.Id);
            var result = await mediator.Send(command);

            result.Should().BeTrue();

            eventPublisher.PublishedEvents
                .OfType<SaleCancelledDomainEvent>()
                .Should().ContainSingle(e => e.SaleId == sale.Id && e.Reason == "Deleted via API");
        }

        [Fact]
        public async Task DeleteSale_Should_ThrowNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            var mediator = _provider.GetRequiredService<IMediator>();
            var nonExistentId = Guid.NewGuid();
            var command = new DeleteSaleCommand(nonExistentId);

            // Act
            Func<Task> act = async () => await mediator.Send(command);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"*{nonExistentId}*");

        }

    }
}