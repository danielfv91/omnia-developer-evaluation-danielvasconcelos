using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Handlers;

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

    [Theory]
    [MemberData(nameof(SalesHandlersTestData.DiscountScenarios), MemberType = typeof(SalesHandlersTestData))]
    public async Task Handle_Should_UpdateSale_And_Publish_SaleModifiedEvent(
        int quantity, decimal unitPrice, decimal discount, decimal expectedTotal)
    {
        var saleId = Guid.NewGuid();
        var command = SaleFakerBuilder.CreateValidUpdateCommand(saleId, quantity, unitPrice);

        var existing = Sale.Create(123, DateTime.UtcNow.AddDays(-1), command.CustomerId, "Daniel", "Branch", new List<SaleItemInput>());
        typeof(Sale).GetProperty(nameof(Sale.Id))!.SetValue(existing, saleId);

        _saleRepository.GetByIdAsync(saleId).Returns(existing);

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        var result = await handler.Handle(command, CancellationToken.None);

        result.TotalAmount.Should().BeApproximately(expectedTotal, 0.01m);

        await _eventPublisher.Received(1).PublishAsync(Arg.Is<SaleModifiedDomainEvent>(e =>
            e.SaleId == saleId &&
            e.TotalAmount == expectedTotal));
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_WhenSaleNotExists()
    {
        var command = SaleFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 5, 10);
        _saleRepository.GetByIdAsync(command.Id).Returns((Sale?)null);

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();

        await _eventPublisher.DidNotReceive().PublishAsync(Arg.Any<SaleModifiedDomainEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessException_WhenQuantityInvalid()
    {
        var saleId = Guid.NewGuid();
        var command = SaleFakerBuilder.CreateValidUpdateCommand(saleId, 30, 10);

        var sale = Sale.Create(1001, DateTime.UtcNow, command.CustomerId, command.CustomerName, command.Branch, new List<SaleItemInput>());
        typeof(Sale).GetProperty(nameof(Sale.Id))!.SetValue(sale, saleId);

        _saleRepository.GetByIdAsync(saleId).Returns(sale);

        var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BusinessException>();

        await _eventPublisher.DidNotReceive().PublishAsync(Arg.Any<SaleModifiedDomainEvent>(), Arg.Any<CancellationToken>());
    }
}
