using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Integration.Builders;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.Integration.Mocks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers
{

    public class CreateSaleHandlerTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IServiceProvider _provider;

        public CreateSaleHandlerTests(IntegrationTestFixture fixture)
        {
            _provider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task CreateSale_Should_Persist_With_Discount_And_DomainEvent()
        {
            var mediator = _provider.GetRequiredService<IMediator>();
            var eventPublisher = _provider.GetRequiredService<IEventPublisher>() as IEventPublisherMock;
            eventPublisher!.Reset();

            var command = SaleCommandFakerBuilder.BuildValidSale(quantity: 5, unitPrice: 10m);

            // Act
            var result = await mediator.Send(command);

            // Assert
            result.Should().NotBeNull();
            result.TotalAmount.Should().Be(45);

            eventPublisher.PublishedEvents
                .OfType<SaleCreatedDomainEvent>()
                .Should()
                .ContainSingle(e =>
                    e.TotalAmount == 45 &&
                    e.CustomerName == command.CustomerName);

        }

        [Fact]
        public async Task CreateSale_Should_Apply_20Percent_Discount_When_Quantity_Is_10()
        {
            var mediator = _provider.GetRequiredService<IMediator>();
            var command = SaleCommandFakerBuilder.BuildValidSale(quantity: 10, unitPrice: 20m);

            var result = await mediator.Send(command);

            result.TotalAmount.Should().Be(160);
        }

        [Fact]
        public async Task CreateSale_Should_NotApplyDiscount_When_Quantity_IsLessThan4()
        {
            var mediator = _provider.GetRequiredService<IMediator>();
            var command = SaleCommandFakerBuilder.BuildValidSale(quantity: 2, unitPrice: 10m);

            var result = await mediator.Send(command);

            result.TotalAmount.Should().Be(20);
        }

        [Fact]
        public async Task CreateSale_Should_Allow_UpperLimit_Boundary_Of_20()
        {
            var mediator = _provider.GetRequiredService<IMediator>();
            var command = SaleCommandFakerBuilder.BuildValidSale(quantity: 20, unitPrice: 10m);

            var result = await mediator.Send(command);

            result.TotalAmount.Should().Be(160);
        }

        [Fact]
        public async Task CreateSale_Should_ThrowValidation_When_Quantity_ExceedsLimit()
        {
            var mediator = _provider.GetRequiredService<IMediator>();
            var command = SaleCommandFakerBuilder.BuildSaleWithInvalidQuantity(21);

            Func<Task> act = async () => await mediator.Send(command);

            await act.Should().ThrowAsync<ValidationException>()
                .Where(ex => ex.Errors.Any(e => e.ErrorMessage.Contains("between 1 and 20")));
        }

        [Fact]
        public async Task CreateSale_Should_ThrowValidation_When_RequiredFieldsAreInvalid()
        {
            var mediator = _provider.GetRequiredService<IMediator>();
            var command = new CreateSaleCommand
            {
                SaleNumber = 1234,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.Empty,
                CustomerName = "",
                Branch = "",
                Items = new List<CreateSaleItemDto>()
            };

            Func<Task> act = async () => await mediator.Send(command);

            await act.Should().ThrowAsync<ValidationException>()
                .Where(ex => ex.Errors.Any());
        }

        [Fact]
        public async Task CreateSale_Should_Handle_MultipleItems_With_Discounts()
        {
            var mediator = _provider.GetRequiredService<IMediator>();

            var command = SaleCommandFakerBuilder.BuildSaleWithMultipleItems(
                (3, 10m),
                (5, 20m),
                (10, 15m)
            );

            var result = await mediator.Send(command);

            result.TotalAmount.Should().Be(240);
        }
    }
}