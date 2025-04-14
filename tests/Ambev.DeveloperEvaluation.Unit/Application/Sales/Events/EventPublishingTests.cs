using Ambev.DeveloperEvaluation.Application.Events.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Sales.Events;

public class EventPublishingTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly IMapper _mapper;

    public EventPublishingTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateSaleProfile>();
            cfg.CreateMap<Sale, UpdateSaleResult>();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task CreateSale_Should_Publish_SaleCreatedDomainEvent()
    {
        // Arrange
        var command = SaleFakerBuilder.CreateValidCreateCommand(1);
        var handler = new CreateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        Sale? capturedSale = null;
        await _saleRepository.AddAsync(Arg.Do<Sale>(s => capturedSale = s), Arg.Any<CancellationToken>());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        capturedSale.Should().NotBeNull();

        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<SaleCreatedDomainEvent>(e =>
                e.SaleId == capturedSale!.Id &&
                e.SaleNumber == capturedSale.SaleNumber &&
                e.TotalAmount == capturedSale.TotalAmount),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateSale_Should_Publish_SaleModifiedDomainEvent()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var quantity = 5;
        var unitPrice = 10m;

        var command = SaleFakerBuilder.CreateValidUpdateCommand(saleId, quantity, unitPrice);

        var items = command.Items.Select(i => new SaleItemInput
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        });

        var sale = Sale.Create(123, DateTime.UtcNow, Guid.NewGuid(), "Cliente", "Branch", items);
        typeof(Sale).GetProperty(nameof(Sale.Id))!.SetValue(sale, saleId);

        _saleRepository.GetByIdAsync(saleId).Returns(sale);
        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<SaleModifiedDomainEvent>(e =>
                e.SaleId == saleId &&
                e.SaleNumber == command.SaleNumber &&
                e.TotalAmount == sale.TotalAmount),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateSale_Should_Publish_ItemCancelledDomainEvent_WhenItemRemoved()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var removedProductId = Guid.NewGuid();

        var originalItems = new List<SaleItemInput>
        {
            new() { ProductId = removedProductId, ProductName = "P1", Quantity = 1, UnitPrice = 10 },
            new() { ProductId = Guid.NewGuid(), ProductName = "P2", Quantity = 2, UnitPrice = 20 }
        };

        var sale = Sale.Create(123, DateTime.UtcNow, Guid.NewGuid(), "Cliente", "Branch", originalItems);
        typeof(Sale).GetProperty(nameof(Sale.Id))!.SetValue(sale, saleId);

        _saleRepository.GetByIdAsync(saleId).Returns(sale);

        var remainingItem = originalItems.Last();
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            SaleNumber = 123,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            Branch = "Branch",
            Items = new List<UpdateSaleItemDto>
            {
                new()
                {
                    ProductId = remainingItem.ProductId,
                    ProductName = remainingItem.ProductName,
                    Quantity = remainingItem.Quantity,
                    UnitPrice = remainingItem.UnitPrice
                }
            }
        };

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        await _eventPublisher.Received().PublishAsync(
            Arg.Is<ItemCancelledDomainEvent>(e =>
                e.SaleId == saleId &&
                e.ProductId == removedProductId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteSale_Should_Publish_SaleCancelledDomainEvent()
    {
        // Arrange
        var sale = Sale.Create(1001, DateTime.UtcNow, Guid.NewGuid(), "Cliente", "Branch", new List<SaleItemInput>());
        _saleRepository.GetByIdAsync(sale.Id).Returns(sale);

        var command = new DeleteSaleCommand(sale.Id);
        var handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<SaleCancelledDomainEvent>(e => e.SaleId == sale.Id),
            Arg.Any<CancellationToken>());
    }
}
