using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class CreateSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly CreateSaleHandler _handler;
        private readonly IEventPublisher _eventPublisher;

        public CreateSaleHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _mapper = Substitute.For<IMapper>();
            _eventPublisher = Substitute.For<IEventPublisher>();
            _handler = new CreateSaleHandler(_saleRepository, _mapper, _eventPublisher);
        }


        [Fact]
        public async Task Handle_Should_CreateSaleSuccessfully()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleNumber = 1,
                SaleDate = DateTime.Now,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Customer Test",
                Branch = "Branch Test",
                Items = new List<CreateSaleItemDto>
            {
                new() { ProductId = Guid.NewGuid(), ProductName = "Product 1", Quantity = 3, UnitPrice = 10 },
                new() { ProductId = Guid.NewGuid(), ProductName = "Product 2", Quantity = 18, UnitPrice = 25 }
            }
            };

            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = command.SaleNumber,
                CustomerName = command.CustomerName,
                Items = command.Items.Select(item => new SaleItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            _mapper.Map<Sale>(command).Returns(sale);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            await _saleRepository.Received(1).AddAsync(Arg.Is<Sale>(s =>
                s.SaleNumber == command.SaleNumber &&
                s.CustomerName == command.CustomerName &&
                s.Items.Count == command.Items.Count &&
                s.TotalAmount > 0
            ));

            Assert.NotNull(result);
            Assert.Equal(sale.Id, result.Id);
            Assert.Equal(sale.SaleNumber, result.SaleNumber);
            Assert.Equal(sale.TotalAmount, result.TotalAmount);
        }
    }
}
