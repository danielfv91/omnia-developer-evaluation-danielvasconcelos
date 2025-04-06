using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Builders;
using Ambev.DeveloperEvaluation.Domain.Entities;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class DeleteSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
        private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();

        [Fact]
        public async Task Handle_Should_DeleteSale_WhenSaleExists()
        {
            // Arrange
            var existingSale = SaleFakerBuilder.GenerateValidSale();
            _saleRepository.GetByIdAsync(existingSale.Id).Returns(existingSale);

            var command = new DeleteSaleCommand(existingSale.Id);
            var handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            await _saleRepository.Received(1).DeleteAsync(existingSale.Id);
        }

        [Fact]
        public async Task Handle_Should_ReturnFalse_WhenSaleDoesNotExist()
        {
            // Arrange
            var fakeId = Guid.NewGuid();

            _saleRepository.GetByIdAsync(fakeId)
                   .Returns(ci => Task.FromResult<Sale>(null!));

            var command = new DeleteSaleCommand(fakeId);
            var handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            await _saleRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
        }
    }
}
