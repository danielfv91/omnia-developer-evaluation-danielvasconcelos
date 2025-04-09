using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers;

public class DeleteSaleHandlerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IServiceProvider _provider;

    public DeleteSaleHandlerTests(IntegrationTestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task DeleteSale_Should_DeleteExistingSale_AndPublishEvent()
    {
        // Arrange
        var context = _provider.GetRequiredService<DefaultContext>();
        var publisher = Substitute.For<IEventPublisher>();
        var mediator = BuildMediator(publisher);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = 1234,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer A",
            Branch = "Branch A",
            Items = new List<SaleItem>()
        };

        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();

        var command = new DeleteSaleCommand(sale.Id);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.Should().BeTrue();

        await publisher.Received(1).PublishAsync(Arg.Is<SaleCancelledEvent>(e =>
            e.SaleId == sale.Id && e.Reason == "Deleted via API"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteSale_Should_ReturnFalse_WhenSaleDoesNotExist()
    {
        var publisher = Substitute.For<IEventPublisher>();
        var mediator = BuildMediator(publisher);

        var command = new DeleteSaleCommand(Guid.NewGuid());

        var result = await mediator.Send(command);

        result.Should().BeFalse();

        await publisher.DidNotReceive().PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    private IMediator BuildMediator(IEventPublisher publisher)
    {
        var services = new ServiceCollection();

        services.AddSingleton(_provider.GetRequiredService<DefaultContext>());
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped(_ => publisher);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<DeleteSaleCommand>());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
}
