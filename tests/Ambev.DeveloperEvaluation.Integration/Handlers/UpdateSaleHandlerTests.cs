using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Integration.Builders;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.Integration.Mocks;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers
{

    public class UpdateSaleHandlerTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IServiceProvider _provider;

        public UpdateSaleHandlerTests(IntegrationTestFixture fixture)
        {
            _provider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task UpdateSale_Should_Update_WithDiscount_And_RaiseDomainEvent()
        {
            var context = _provider.GetRequiredService<DefaultContext>();
            var mediator = _provider.GetRequiredService<IMediator>();
            var eventPublisher = _provider.GetRequiredService<IEventPublisher>() as IEventPublisherMock;
            eventPublisher!.Reset();

            var sale = SaleUpdateFakerBuilder.CreateValidEntity(5, 10m);
            await context.Sales.AddAsync(sale);
            await context.SaveChangesAsync();

            var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(sale.Id, 10, 20m);
            var result = await mediator.Send(command);

            result.TotalAmount.Should().Be(160);

            eventPublisher.PublishedEvents
                .OfType<SaleModifiedDomainEvent>()
                .Should().ContainSingle(e =>
                    e.SaleId == sale.Id &&
                    e.TotalAmount == 160);
        }

        [Fact]
        public async Task UpdateSale_Should_NotApplyDiscount_When_Quantity_IsLessThan4()
        {
            var context = _provider.GetRequiredService<DefaultContext>();
            var mediator = _provider.GetRequiredService<IMediator>();

            var sale = SaleUpdateFakerBuilder.CreateValidEntity(3, 10m);
            await context.Sales.AddAsync(sale);
            await context.SaveChangesAsync();

            var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(sale.Id, 2, 10m);

            var result = await mediator.Send(command);

            result.TotalAmount.Should().Be(20);
        }

        [Fact]
        public async Task UpdateSale_Should_Apply_20PercentDiscount_When_Quantity_Is_10()
        {
            var context = _provider.GetRequiredService<DefaultContext>();
            var mediator = _provider.GetRequiredService<IMediator>();

            var sale = SaleUpdateFakerBuilder.CreateValidEntity(3, 10m);
            await context.Sales.AddAsync(sale);
            await context.SaveChangesAsync();

            var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(sale.Id, 10, 10m);

            var result = await mediator.Send(command);

            result.TotalAmount.Should().Be(80);
        }

        [Fact]
        public async Task UpdateSale_Should_ThrowValidationException_When_Quantity_Exceeds_20()
        {
            var mediator = _provider.GetRequiredService<IMediator>();

            var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 21, 10m);

            Func<Task> act = async () => await mediator.Send(command);

            await act.Should().ThrowAsync<ValidationException>()
                .Where(ex => ex.Errors.Any(e => e.ErrorMessage.Contains("Cannot sell more than 20 identical items")));
        }

        [Fact]
        public async Task UpdateSale_Should_Throw_NotFoundException_When_Sale_DoesNotExist()
        {
            var mediator = _provider.GetRequiredService<IMediator>();

            var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 1, 10m);

            Func<Task> act = async () => await mediator.Send(command);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*not found*");
        }

        [Fact]
        public async Task UpdateSale_Should_Raise_ItemCancelledDomainEvent_When_Item_Removed()
        {
            var context = _provider.GetRequiredService<DefaultContext>();
            var mediator = _provider.GetRequiredService<IMediator>();
            var eventPublisher = _provider.GetRequiredService<IEventPublisher>() as IEventPublisherMock;
            eventPublisher!.Reset();

            var sale = SaleUpdateFakerBuilder.CreateValidEntity(3, 10m);
            var removedItem = sale.Items.First();

            await context.Sales.AddAsync(sale);
            await context.SaveChangesAsync();

            var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(sale.Id, 1, 10m);
            command.Items[0].ProductId = Guid.NewGuid();

            await mediator.Send(command);

            eventPublisher.PublishedEvents
                .OfType<ItemCancelledDomainEvent>()
                .Should().ContainSingle(e => e.ProductId == removedItem.ProductId);
        }

    }
}