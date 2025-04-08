using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Handlers
{
    public class CreateSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
        private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
        private readonly ISaleItemBuilder _itemBuilder = Substitute.For<ISaleItemBuilder>();
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
        public async Task Handle_Should_CreateSaleCorrectly_WithCalculatedTotal(int quantity, decimal unitPrice, decimal discount, decimal expectedTotal)
        {
            // Arrange
            var command = SalesHandlersTestData.CreateCommand(quantity, unitPrice);

            var expectedItems = new List<SaleItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SaleId = Guid.NewGuid(),
                    ProductId = command.Items[0].ProductId,
                    ProductName = command.Items[0].ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountPercentage = discount,
                    TotalItemAmount = expectedTotal,
                    IsCancelled = false
                }
            };

            _itemBuilder.Build(command.Items, Arg.Any<Guid>()).Returns(expectedItems);
            _itemBuilder.CalculateTotalAmount(expectedItems).Returns(expectedTotal);

            var handler = new CreateSaleHandler(
                _saleRepository,
                _mapper,
                _eventPublisher,
                _itemBuilder
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalAmount.Should().Be(expectedTotal);
            await _saleRepository.Received(1).AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
            await _eventPublisher.Received(1).PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
        }
    }
}
