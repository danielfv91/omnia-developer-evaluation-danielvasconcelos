using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using Ambev.DeveloperEvaluation.Integration.Builders;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers;

public class UpdateSaleHandlerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IServiceProvider _provider;

    public UpdateSaleHandlerTests(IntegrationTestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task UpdateSale_Should_UpdateSaleSuccessfully_WithDiscountApplied()
    {
        var context = _provider.GetRequiredService<DefaultContext>();
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var sale = SaleUpdateFakerBuilder.CreateValidEntity(5, 10m);
        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();

        var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(sale.Id, 10, 20m);

        var result = await mediator.Send(command);

        result.TotalAmount.Should().Be(160);

        await publisher.Received(1).PublishAsync(Arg.Is<SaleModifiedEvent>(e =>
            e.SaleId == sale.Id &&
            e.TotalAmount == 160), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateSale_Should_NotApplyDiscount_WhenQuantityIsBelowFour()
    {
        var context = _provider.GetRequiredService<DefaultContext>();
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var sale = SaleUpdateFakerBuilder.CreateValidEntity(1, 10m);
        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();

        var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(sale.Id, 2, 10m);

        var result = await mediator.Send(command);

        result.TotalAmount.Should().Be(20);
    }

    [Fact]
    public async Task UpdateSale_Should_ApplyTwentyPercentDiscount_WhenQuantityIsAtLeastTen()
    {
        var context = _provider.GetRequiredService<DefaultContext>();
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var sale = SaleUpdateFakerBuilder.CreateValidEntity(1, 10m);
        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();

        var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(sale.Id, 10, 10m);

        var result = await mediator.Send(command);

        result.TotalAmount.Should().Be(80);
    }

    [Fact]
    public async Task UpdateSale_Should_ThrowValidationException_WhenQuantityExceedsTwenty()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 21, 10m);

        Func<Task> act = async () => await mediator.Send(command);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.ErrorMessage.Contains("Cannot sell more than 20")));
    }


    [Fact]
    public async Task UpdateSale_Should_ThrowNotFoundException_WhenSaleDoesNotExist()
    {
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 1, 10m);

        Func<Task> act = async () => await mediator.Send(command);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task UpdateSale_Should_PublishItemCancelledEvent_WhenItemIsRemoved()
    {
        var context = _provider.GetRequiredService<DefaultContext>();
        var publisher = Substitute.For<IEventPublisher>();
        var itemBuilder = _provider.GetRequiredService<ISaleItemBuilder>();
        var mediator = BuildMediator(publisher, itemBuilder);

        var sale = SaleUpdateFakerBuilder.CreateValidEntity(3, 10m);
        var originalItem = sale.Items.First();

        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();

        var command = SaleUpdateFakerBuilder.CreateValidUpdateCommand(sale.Id, 1, 10m);
        command.Items[0].ProductId = Guid.NewGuid();

        await mediator.Send(command);

        await publisher.Received(1).PublishAsync(Arg.Is<ItemCancelledEvent>(e =>
            e.ProductName == originalItem.ProductName), Arg.Any<CancellationToken>());
    }

    private IMediator BuildMediator(IEventPublisher publisher, ISaleItemBuilder itemBuilder)
    {
        var services = new ServiceCollection();

        services.AddSingleton(_provider.GetRequiredService<DefaultContext>());
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped(_ => publisher);
        services.AddScoped(_ => itemBuilder);
        services.AddAutoMapper(typeof(UpdateSaleCommand).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UpdateSaleCommand>());
        services.AddValidatorsFromAssemblyContaining<UpdateSaleValidator>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
}
