using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Handlers;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();

    [Fact]
    public async Task Handle_Should_CancelSale_And_Publish_SaleCancelledEvent()
    {
        var sale = SaleFakerBuilder.GenerateValidSale();
        _saleRepository.GetByIdAsync(sale.Id).Returns(sale);

        var handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);
        var result = await handler.Handle(new DeleteSaleCommand(sale.Id), CancellationToken.None);

        result.Should().BeTrue();

        await _eventPublisher.Received(1).PublishAsync(Arg.Is<SaleCancelledDomainEvent>(e =>
            e.SaleId == sale.Id &&
            e.Reason == "Deleted via API"));
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_WhenSaleMissing()
    {
        var id = Guid.NewGuid();
        _saleRepository.GetByIdAsync(id).Returns((Sale?)null);

        var handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);
        Func<Task> act = async () => await handler.Handle(new DeleteSaleCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await _eventPublisher.DidNotReceive().PublishAsync(Arg.Any<SaleCancelledDomainEvent>(), Arg.Any<CancellationToken>());
    }
}
