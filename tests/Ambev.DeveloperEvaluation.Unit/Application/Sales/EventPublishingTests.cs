﻿using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Builders;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class EventPublishingTests
    {
        private readonly ISaleRepository _repository = Substitute.For<ISaleRepository>();
        private readonly IEventPublisher _publisher = Substitute.For<IEventPublisher>();

        [Fact]
        public async Task CreateSale_Should_PublishSaleCreatedEvent()
        {
            // Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateSaleItemDto, SaleItem>();
                cfg.CreateMap<CreateSaleCommand, Sale>();
                cfg.CreateMap<Sale, CreateSaleResult>();
            }).CreateMapper();

            var command = SaleFakerBuilder.CreateValidCreateCommand(2);
            var handler = new CreateSaleHandler(_repository, mapper, _publisher);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await _publisher.Received(1).PublishAsync(Arg.Is<SaleCreatedEvent>(e =>
                e.SaleNumber == command.SaleNumber &&
                e.CustomerName == command.CustomerName &&
                e.TotalAmount > 0));
        }

        [Fact]
        public async Task UpdateSale_Should_PublishSaleModifiedEvent()
        {
            // Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Sale, UpdateSaleResult>();
            }).CreateMapper();

            var saleId = Guid.NewGuid();
            var existingSale = new Sale { Id = saleId, Items = new List<SaleItem>() };
            _repository.GetByIdAsync(saleId).Returns(existingSale);

            var command = SaleFakerBuilder.CreateValidUpdateCommand(saleId, 1);
            var handler = new UpdateSaleHandler(_repository, mapper, _publisher);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await _publisher.Received(1).PublishAsync(Arg.Is<SaleModifiedEvent>(e =>
                e.SaleNumber == command.SaleNumber &&
                e.TotalAmount > 0));
        }

        [Fact]
        public async Task DeleteSale_Should_PublishSaleCancelledEvent()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var existingSale = new Sale { Id = saleId };
            _repository.GetByIdAsync(saleId).Returns(existingSale);

            var handler = new DeleteSaleHandler(_repository, _publisher);

            // Act
            await handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

            // Assert
            await _publisher.Received(1).PublishAsync(Arg.Is<SaleCancelledEvent>(e =>
                e.SaleId == saleId &&
                e.Reason == "Deleted via API"));
        }
    }
}
