using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.Events;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class EventPublishingTests
{
    [Fact]
    public async Task CreateSale_Should_PublishSaleCreatedEvent()
    {
        var repository = Substitute.For<ISaleRepository>();
        var publisher = Substitute.For<IEventPublisher>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateSaleItemDto, SaleItem>();
            cfg.CreateMap<CreateSaleCommand, Sale>();
            cfg.CreateMap<Sale, CreateSaleResult>();
        });

        var mapper = config.CreateMapper();

        var handler = new CreateSaleHandler(repository, mapper, publisher);

        var command = new CreateSaleCommand
        {
            SaleNumber = 2001,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente A",
            Branch = "Filial A",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto X",
                    Quantity = 5,
                    UnitPrice = 10
                }
            }
        };

        await handler.Handle(command, CancellationToken.None);

        await publisher.Received(1).PublishAsync(Arg.Is<SaleCreatedEvent>(e =>
            e.SaleNumber == 2001 &&
            e.CustomerName == "Cliente A" &&
            e.TotalAmount > 0));
    }

    [Fact]
    public async Task UpdateSale_Should_PublishSaleModifiedEvent()
    {
        var saleId = Guid.NewGuid();
        var repository = Substitute.For<ISaleRepository>();
        var publisher = Substitute.For<IEventPublisher>();

        repository.GetByIdAsync(saleId).Returns(new Sale { Id = saleId, Items = new List<SaleItem>() });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Sale, UpdateSaleResult>();
        });
        var mapper = config.CreateMapper();

        var handler = new UpdateSaleHandler(repository, mapper, publisher);

        var command = new UpdateSaleCommand
        {
            Id = saleId,
            SaleNumber = 2002,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente B",
            Branch = "Filial B",
            Items = new List<UpdateSaleItemDto>
            {
                new UpdateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Y",
                    Quantity = 4,
                    UnitPrice = 20
                }
            }
        };

        await handler.Handle(command, CancellationToken.None);

        await publisher.Received(1).PublishAsync(Arg.Is<SaleModifiedEvent>(e =>
            e.SaleNumber == 2002 &&
            e.TotalAmount > 0));
    }

    [Fact]
    public async Task DeleteSale_Should_PublishSaleCancelledEvent()
    {
        var saleId = Guid.NewGuid();
        var repository = Substitute.For<ISaleRepository>();
        var publisher = Substitute.For<IEventPublisher>();

        repository.GetByIdAsync(saleId).Returns(new Sale { Id = saleId });

        var handler = new DeleteSaleHandler(repository, publisher);

        await handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        await publisher.Received(1).PublishAsync(Arg.Is<SaleCancelledEvent>(e =>
            e.SaleId == saleId &&
            e.Reason == "Deleted via API"));
    }
}