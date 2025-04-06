using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Builders;
using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly IMapper _mapper;

    public UpdateSaleHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Sale, UpdateSaleResult>();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_Should_UpdateSaleSuccessfully()
    {
        // Arrange
        var command = SaleFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), items: 2);
        var existingSale = new Sale { Id = command.Id, Items = new List<SaleItem>() };

        _saleRepository.GetByIdAsync(command.Id).Returns(existingSale);

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.SaleNumber, result.SaleNumber);
        Assert.Equal(command.CustomerName, result.CustomerName);
        Assert.Equal(command.Branch, result.Branch);
        Assert.True(result.TotalAmount > 0);

        await _saleRepository.Received(1).UpdateAsync(Arg.Is<Sale>(s =>
            s.SaleNumber == command.SaleNumber &&
            s.CustomerName == command.CustomerName &&
            s.Items.Count == command.Items.Count &&
            s.TotalAmount > 0));
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessException_WhenQuantityExceedsLimit()
    {
        // Arrange
        var command = SaleFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 1);
        command.Items[0].Quantity = 25;

        var existingSale = new Sale { Id = command.Id, Items = new List<SaleItem>() };
        _saleRepository.GetByIdAsync(command.Id).Returns(existingSale);

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(command, CancellationToken.None));
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
    }

    [Fact]
    public async Task Handle_Should_CalculateNoDiscount_WhenQuantityIsLessThan4()
    {
        // Arrange
        var command = SaleFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 1);
        command.Items[0].Quantity = 2;
        command.Items[0].UnitPrice = 10;

        var expectedTotal = 2 * 10;
        var existingSale = new Sale { Id = command.Id, Items = new List<SaleItem>() };

        _saleRepository.GetByIdAsync(command.Id).Returns(existingSale);

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTotal, result.TotalAmount);
    }

    [Fact]
    public async Task Handle_Should_Apply10PercentDiscount_WhenQuantityBetween4And9()
    {
        // Arrange
        var command = SaleFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 1);
        command.Items[0].Quantity = 5;
        command.Items[0].UnitPrice = 20;

        var expectedTotal = 5 * 20 * 0.9m;
        var existingSale = new Sale { Id = command.Id, Items = new List<SaleItem>() };

        _saleRepository.GetByIdAsync(command.Id).Returns(existingSale);

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTotal, result.TotalAmount);
    }

    [Fact]
    public async Task Handle_Should_Apply20PercentDiscount_WhenQuantityBetween10And20()
    {
        // Arrange
        var command = SaleFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 1);
        command.Items[0].Quantity = 15;
        command.Items[0].UnitPrice = 30;

        var expectedTotal = 15 * 30 * 0.8m;
        var existingSale = new Sale { Id = command.Id, Items = new List<SaleItem>() };

        _saleRepository.GetByIdAsync(command.Id).Returns(existingSale);

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTotal, result.TotalAmount);
    }
}
