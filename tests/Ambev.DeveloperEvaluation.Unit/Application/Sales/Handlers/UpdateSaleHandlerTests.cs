using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Handlers
{
    public class UpdateSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
        private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
        private readonly ISaleItemBuilder _itemBuilder = Substitute.For<ISaleItemBuilder>();
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
        public async Task Handle_Should_UpdateSale_Correctly_With_Discount(int quantity, decimal unitPrice, decimal discount, decimal expectedTotal)
        {
            // Arrange
            var command = SaleFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 1);
            var item = command.Items[0];
            item.Quantity = quantity;
            item.UnitPrice = unitPrice;

            var existingSale = new Sale { Id = command.Id, Items = new List<SaleItem>() };
            _saleRepository.GetByIdAsync(command.Id).Returns(existingSale);

            var expectedItems = new List<SaleItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SaleId = command.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountPercentage = discount,
                    TotalItemAmount = expectedTotal,
                    IsCancelled = false
                }
            };

            _itemBuilder.Build(command.Items, command.Id).Returns(expectedItems);
            _itemBuilder.CalculateTotalAmount(expectedItems).Returns(expectedTotal);

            var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher, _itemBuilder);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalAmount.Should().BeApproximately(expectedTotal, 0.01m);
            await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowBusinessException_WhenQuantityExceedsLimit()
        {
            // Arrange
            var command = SaleFakerBuilder.CreateValidUpdateCommand(Guid.NewGuid(), 1);
            command.Items[0].Quantity = 25;
            command.Items[0].UnitPrice = 10;

            var existingSale = new Sale { Id = command.Id, Items = new List<SaleItem>() };
            _saleRepository.GetByIdAsync(command.Id).Returns(existingSale);

            _itemBuilder
                .Build(command.Items, command.Id)
                .Throws(new BusinessException("Cannot sell more than 20 identical items per product."));

            var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher, _itemBuilder);

            // Act & Assert
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<BusinessException>();

            await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
        }
    }
}
