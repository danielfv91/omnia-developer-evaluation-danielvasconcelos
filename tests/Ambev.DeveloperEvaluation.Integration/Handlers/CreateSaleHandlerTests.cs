using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using Ambev.DeveloperEvaluation.Integration.Builders;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers;

public class CreateSaleHandlerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IServiceProvider _provider;

    public CreateSaleHandlerTests(IntegrationTestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task CreateSale_Should_PersistAndPublishEvent_WithDiscountApplied()
    {
        // Arrange
        var context = _provider.GetRequiredService<DefaultContext>();
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();

        _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var command = SaleCommandFakerBuilder.BuildValidSale(quantity: 5, unitPrice: 10m);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(45);

        await publisher.Received(1).PublishAsync(Arg.Is<SaleCreatedEvent>(e =>
            e.SaleNumber == command.SaleNumber &&
            e.TotalAmount == 45), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateSale_Should_ApplyTwentyPercentDiscount_WhenQuantityIsAtLeastTen()
    {
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var command = SaleCommandFakerBuilder.BuildValidSale(quantity: 10, unitPrice: 20m);

        var result = await mediator.Send(command);

        result.TotalAmount.Should().Be(160);
    }

    [Fact]
    public async Task CreateSale_Should_NotApplyDiscount_WhenQuantityIsLessThanFour()
    {
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var command = SaleCommandFakerBuilder.BuildValidSale(quantity: 2, unitPrice: 10m);

        var result = await mediator.Send(command);

        result.TotalAmount.Should().Be(20);
    }

    [Fact]
    public async Task CreateSale_Should_AllowMaximumQuantityLimitBoundary()
    {
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var command = SaleCommandFakerBuilder.BuildValidSale(quantity: 20, unitPrice: 10m);

        var result = await mediator.Send(command);

        result.TotalAmount.Should().Be(160);
    }

    [Fact]
    public async Task CreateSale_Should_ThrowValidationException_WhenQuantityExceedsTwenty()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var command = SaleCommandFakerBuilder.BuildSaleWithInvalidQuantity(21);

        Func<Task> act = async () => await mediator.Send(command);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.ErrorMessage.Contains("must be between 1 and 20")));
    }


    [Fact]
    public async Task CreateSale_Should_ThrowValidationException_WhenRequiredFieldsAreMissing()
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
    public async Task CreateSale_Should_HandleMultipleItems_WithCorrectTotalAndDiscounts()
    {
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var command = SaleCommandFakerBuilder.BuildSaleWithMultipleItems(
            (3, 10m),
            (5, 20m),
            (10, 15m)
        );

        var result = await mediator.Send(command);

        result.TotalAmount.Should().Be(240);
    }

    private IMediator BuildMediator(IEventPublisher publisher, ISaleItemBuilder itemBuilder)
    {
        var services = new ServiceCollection();

        services.AddSingleton(_provider.GetRequiredService<DefaultContext>());
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped(_ => publisher);
        services.AddScoped(_ => itemBuilder);
        services.AddAutoMapper(typeof(CreateSaleCommand).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateSaleCommand>());
        services.AddValidatorsFromAssemblyContaining<CreateSaleValidator>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
}
