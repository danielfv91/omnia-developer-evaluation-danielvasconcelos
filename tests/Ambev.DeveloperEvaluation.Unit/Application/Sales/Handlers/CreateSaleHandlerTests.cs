using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Handlers;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly IMapper _mapper;

    public CreateSaleHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateSaleProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Theory]
    [MemberData(nameof(SalesHandlersTestData.DiscountScenarios), MemberType = typeof(SalesHandlersTestData))]
    public async Task Handle_Should_CreateSaleCorrectly_And_PublishEvent(
        int quantity, decimal unitPrice, decimal discount, decimal expectedTotal)
    {
        // Arrange
        var command = SalesHandlersTestData.CreateCommand(quantity, unitPrice);
        var handler = new CreateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().BeApproximately(expectedTotal, 0.01m);

        await _saleRepository.Received(1).AddAsync(Arg.Is<Sale>(s =>
            s.Items.Count == 1 &&
            s.TotalAmount == expectedTotal &&
            s.Items.First().Quantity == quantity &&
            s.Items.First().UnitPrice == unitPrice &&
            s.Items.First().DiscountPercentage == discount), Arg.Any<CancellationToken>());

        await _eventPublisher.Received(1).PublishAsync(Arg.Is<SaleCreatedDomainEvent>(e =>
            e.TotalAmount == expectedTotal &&
            e.CustomerName == command.CustomerName &&
            e.SaleNumber == command.SaleNumber));
    }
}
