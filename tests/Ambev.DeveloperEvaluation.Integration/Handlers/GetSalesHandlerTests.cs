using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Handlers;

public class GetSalesHandlerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IServiceProvider _provider;

    public GetSalesHandlerTests(IntegrationTestFixture fixture)
    {
        _provider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task GetSales_Should_ReturnPaginatedResult()
    {
        var context = _provider.GetRequiredService<DefaultContext>();
        var mediator = _provider.GetRequiredService<IMediator>();

        for (int i = 0; i < 10; i++)
        {
            await context.Sales.AddAsync(new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = 1000 + i,
                SaleDate = DateTime.UtcNow.AddDays(-i),
                CustomerId = Guid.NewGuid(),
                CustomerName = $"Client {i}",
                Branch = "Branch A",
                Items = new List<SaleItem>()
            });
        }

        await context.SaveChangesAsync();

        var query = new GetSalesQuery { Page = 1, Size = 5, Order = "SaleDate desc" };

        var result = await mediator.Send(query);

        result.Should().NotBeNull();
        result.Data.Should().HaveCount(5);
        result.TotalItems.Should().BeGreaterThan(5);
    }

    [Fact]
    public async Task GetSales_Should_ThrowValidationException_WhenPageIsZero()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var query = new GetSalesQuery { Page = 0, Size = 10 };

        Func<Task> act = async () => await mediator.Send(query);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.ErrorMessage.Contains("Page must be greater than 0")));
    }

    [Fact]
    public async Task GetSales_Should_ThrowValidationException_WhenSizeIsOutOfRange()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var query = new GetSalesQuery { Page = 1, Size = 101 };

        Func<Task> act = async () => await mediator.Send(query);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.ErrorMessage.Contains("Size must be between")));
    }

    [Fact]
    public async Task GetSales_Should_ThrowValidationException_WhenDatesAreInFuture()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var future = DateTime.UtcNow.AddDays(5);

        var query = new GetSalesQuery
        {
            Page = 1,
            Size = 10,
            MinDate = future,
            MaxDate = future.AddDays(1)
        };

        Func<Task> act = async () => await mediator.Send(query);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e =>
                e.ErrorMessage.Contains("MinDate cannot be in the future") ||
                e.ErrorMessage.Contains("MaxDate cannot be in the future")));
    }

    [Fact]
    public async Task GetSales_Should_ThrowValidationException_WhenOrderIsInvalid()
    {
        var mediator = _provider.GetRequiredService<IMediator>();
        var query = new GetSalesQuery { Page = 1, Size = 10, Order = "InvalidField" };

        Func<Task> act = async () => await mediator.Send(query);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.ErrorMessage.Contains("Order must contain valid fields")));
    }
}
