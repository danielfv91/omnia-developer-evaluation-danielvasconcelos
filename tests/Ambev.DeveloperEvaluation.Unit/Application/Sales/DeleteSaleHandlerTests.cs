﻿using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{

    public class DeleteSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
        private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();

        [Fact]
        public async Task Handle_Should_DeleteSale_WhenSaleExists()
        {
            var saleId = Guid.NewGuid();
            var existingSale = new Sale { Id = saleId };

            _saleRepository.GetByIdAsync(saleId).Returns(existingSale);

            var command = new DeleteSaleCommand(saleId);
            var handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            await _saleRepository.Received(1).DeleteAsync(saleId);
        }

        [Fact]
        public async Task Handle_Should_ReturnFalse_WhenSaleDoesNotExist()
        {
            var saleId = Guid.NewGuid();
            _saleRepository.GetByIdAsync(saleId).Returns((Sale)null);

            var command = new DeleteSaleCommand(saleId);
            var handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            await _saleRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
        }
    }
}