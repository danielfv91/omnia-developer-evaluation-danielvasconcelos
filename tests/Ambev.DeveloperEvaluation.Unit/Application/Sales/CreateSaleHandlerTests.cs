using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Builders;
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
            var command = SaleFakerBuilder.CreateValidCreateCommand(2);

            var mappedSale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = command.SaleNumber,
                SaleDate = command.SaleDate,
                CustomerId = command.CustomerId,
                CustomerName = command.CustomerName,
                Branch = command.Branch,
                Items = command.Items.Select(i => new SaleItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };

            _mapper.Map<Sale>(command).Returns(mappedSale);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            await _saleRepository.Received(1).AddAsync(Arg.Is<Sale>(s =>
                s.SaleNumber == command.SaleNumber &&
                s.CustomerName == command.CustomerName &&
                s.Branch == command.Branch &&
                s.Items.Count == command.Items.Count &&
                s.TotalAmount > 0
            ));

            Assert.NotNull(result);
            Assert.Equal(mappedSale.Id, result.Id);
            Assert.Equal(mappedSale.SaleNumber, result.SaleNumber);
            Assert.Equal(mappedSale.TotalAmount, result.TotalAmount);
        }
    }
}
