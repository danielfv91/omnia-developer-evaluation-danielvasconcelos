using Ambev.DeveloperEvaluation.Application.Events.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class EventPublishingTests
    {
        private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
        private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
        private readonly ISaleItemBuilder _itemBuilder = Substitute.For<ISaleItemBuilder>();
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

        [Theory]
        [MemberData(nameof(SaleTestData.GenerateWithBogus), MemberType = typeof(SaleTestData))]
        public async Task CreateSale_Should_PublishSaleCreatedEvent_WithRealisticData(int quantity, decimal unitPrice, decimal discount, decimal expectedTotal)
        {
            var command = SaleTestData.CreateCommand(quantity, unitPrice);

            var fakeItem = command.Items.First();

            var expectedItems = new List<SaleItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SaleId = Guid.NewGuid(),
                    ProductId = fakeItem.ProductId,
                    ProductName = fakeItem.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountPercentage = discount,
                    TotalItemAmount = expectedTotal,
                    IsCancelled = false
                }
            };

            _itemBuilder.Build(command.Items, Arg.Any<Guid>()).Returns(expectedItems);
            _itemBuilder.CalculateTotalAmount(expectedItems).Returns(expectedTotal);

            var handler = new CreateSaleHandler(_saleRepository, _mapper, _eventPublisher, _itemBuilder);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.TotalAmount.Should().BeApproximately(expectedTotal, 0.01m);

            await _eventPublisher.Received(1).PublishAsync(Arg.Is<SaleCreatedEvent>(e =>
                e.SaleNumber == command.SaleNumber &&
                e.CustomerName == command.CustomerName &&
                e.TotalAmount == expectedTotal));
        }

        [Theory]
        [MemberData(nameof(SaleTestData.GenerateWithBogus), MemberType = typeof(SaleTestData))]
        public async Task UpdateSale_Should_PublishSaleModifiedEvent_WithRealisticData(int quantity, decimal unitPrice, decimal discount, decimal expectedTotal)
        {
            var saleId = Guid.NewGuid();
            var existingSale = new Sale { Id = saleId, Items = new List<SaleItem>() };
            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var command = SaleFakerBuilder.CreateValidUpdateCommand(saleId, 1);
            var item = command.Items.First();
            item.Quantity = quantity;
            item.UnitPrice = unitPrice;

            var expectedItems = new List<SaleItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SaleId = saleId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountPercentage = discount,
                    TotalItemAmount = expectedTotal,
                    IsCancelled = false
                }
            };

            _itemBuilder.Build(command.Items, saleId).Returns(expectedItems);
            _itemBuilder.CalculateTotalAmount(expectedItems).Returns(expectedTotal);

            var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher, _itemBuilder);

            var result = await handler.Handle(command, CancellationToken.None);

            result.TotalAmount.Should().BeApproximately(expectedTotal, 0.01m);

            await _eventPublisher.Received(1).PublishAsync(Arg.Is<SaleModifiedEvent>(e =>
                e.SaleNumber == command.SaleNumber &&
                e.TotalAmount == expectedTotal));
        }

        [Fact]
        public async Task DeleteSale_Should_PublishSaleCancelledEvent()
        {
            var saleId = Guid.NewGuid();
            var existingSale = new Sale { Id = saleId };
            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);

            await handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

            await _eventPublisher.Received(1).PublishAsync(Arg.Is<SaleCancelledEvent>(e =>
                e.SaleId == saleId &&
                e.Reason == "Deleted via API"));
        }

        [Fact]
        public async Task UpdateSale_Should_PublishItemCancelledEvents_WhenItemsAreRemoved()
        {
            var saleId = Guid.NewGuid();
            var faker = new Faker();

            var originalItems = new List<SaleItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SaleId = saleId,
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 3,
                    UnitPrice = 10,
                    DiscountPercentage = 0,
                    TotalItemAmount = 30,
                    IsCancelled = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    SaleId = saleId,
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 5,
                    UnitPrice = 20,
                    DiscountPercentage = 0.1m,
                    TotalItemAmount = 90,
                    IsCancelled = false
                }
            };

            var existingSale = new Sale { Id = saleId, Items = originalItems };
            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var command = SaleFakerBuilder.CreateValidUpdateCommand(saleId, 1);
            var newItem = command.Items[0];

            var newItems = new List<SaleItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SaleId = saleId,
                    ProductId = newItem.ProductId,
                    ProductName = newItem.ProductName,
                    Quantity = newItem.Quantity,
                    UnitPrice = newItem.UnitPrice,
                    DiscountPercentage = 0,
                    TotalItemAmount = newItem.Quantity * newItem.UnitPrice,
                    IsCancelled = false
                }
            };

            _itemBuilder.Build(command.Items, saleId).Returns(newItems);
            _itemBuilder.CalculateTotalAmount(newItems).Returns(newItems[0].TotalItemAmount);

            var handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher, _itemBuilder);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await _eventPublisher.Received(2).PublishAsync(Arg.Is<ItemCancelledEvent>(e =>
                originalItems.Any(oi => oi.ProductName == e.ProductName)));
        }

    }
}
