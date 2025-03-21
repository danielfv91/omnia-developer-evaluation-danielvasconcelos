using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class UpdateSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
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
            var saleId = Guid.NewGuid();
            var existingSale = new Sale
            {
                Id = saleId,
                SaleNumber = 1001,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                Branch = "Branch A",
                Items = new List<SaleItem>()
            };

            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var command = new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = 2002,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Updated Customer",
                Branch = "Branch B",
                Items = new List<UpdateSaleItemDto>
            {
                new UpdateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 10
                }
            }
            };

            var handler = new UpdateSaleHandler(_saleRepository, _mapper);

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
                s.Items.Count == 1 &&
                s.TotalAmount > 0));
        }

        [Fact]
        public async Task Handle_Should_ThrowBusinessException_WhenQuantityExceedsLimit()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var existingSale = new Sale
            {
                Id = saleId,
                SaleNumber = 1001,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                Branch = "Branch A",
                Items = new List<SaleItem>()
            };

            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var command = new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = 2002,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Updated Customer",
                Branch = "Branch B",
                Items = new List<UpdateSaleItemDto>
            {
                new UpdateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product B",
                    Quantity = 21,
                    UnitPrice = 10
                }
            }
            };

            var handler = new UpdateSaleHandler(_saleRepository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(command, CancellationToken.None));
            await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
        }

        [Fact]
        public async Task Handle_Should_CalculateNoDiscount_WhenQuantityIsLessThan4()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var existingSale = new Sale { Id = saleId, Items = new List<SaleItem>() };
            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var command = new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = 3001,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Customer",
                Branch = "Branch X",
                Items = new List<UpdateSaleItemDto>
        {
            new UpdateSaleItemDto
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Product 1",
                Quantity = 3,
                UnitPrice = 10
            }
        }
            };

            var handler = new UpdateSaleHandler(_saleRepository, _mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var expectedTotal = 3 * 10;
            Assert.Equal(expectedTotal, result.TotalAmount);
        }

        [Fact]
        public async Task Handle_Should_Apply10PercentDiscount_WhenQuantityBetween4And9()
        {
            var saleId = Guid.NewGuid();
            var existingSale = new Sale { Id = saleId, Items = new List<SaleItem>() };
            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var command = new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = 3002,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Customer",
                Branch = "Branch X",
                Items = new List<UpdateSaleItemDto>
        {
            new UpdateSaleItemDto
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Product 2",
                Quantity = 5,
                UnitPrice = 20
            }
        }
            };

            var handler = new UpdateSaleHandler(_saleRepository, _mapper);

            var result = await handler.Handle(command, CancellationToken.None);

            var totalBeforeDiscount = 5 * 20;
            var expectedTotal = totalBeforeDiscount * 0.90m;

            Assert.Equal(expectedTotal, result.TotalAmount);
        }

        [Fact]
        public async Task Handle_Should_Apply20PercentDiscount_WhenQuantityBetween10And20()
        {
            var saleId = Guid.NewGuid();
            var existingSale = new Sale { Id = saleId, Items = new List<SaleItem>() };
            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var command = new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = 3003,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Customer",
                Branch = "Branch X",
                Items = new List<UpdateSaleItemDto>
        {
            new UpdateSaleItemDto
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Product 3",
                Quantity = 15,
                UnitPrice = 30
            }
        }
            };

            var handler = new UpdateSaleHandler(_saleRepository, _mapper);

            var result = await handler.Handle(command, CancellationToken.None);

            var totalBeforeDiscount = 15 * 30;
            var expectedTotal = totalBeforeDiscount * 0.80m;

            Assert.Equal(expectedTotal, result.TotalAmount);
        }

    }
}