using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Builders;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class GetSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly GetSaleHandler _handler;

        public GetSaleHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Sale, GetSaleResult>();
                cfg.CreateMap<SaleItem, GetSaleItemDto>();
            });
            _mapper = config.CreateMapper();

            _handler = new GetSaleHandler(_saleRepository, _mapper);
        }

        [Fact]
        public async Task Handle_Should_ReturnSale_WhenSaleExists()
        {
            // Arrange
            var fakeSale = SaleFakerBuilder.GenerateValidSaleWithItems(2);
            _saleRepository.GetByIdAsync(fakeSale.Id).Returns(fakeSale);

            var query = new GetSaleQuery { Id = fakeSale.Id };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeSale.SaleNumber, result.SaleNumber);
            Assert.Equal(fakeSale.CustomerName, result.CustomerName);
            Assert.Equal(fakeSale.Items.Count, result.Items.Count);
        }

        [Fact]
        public async Task Handle_Should_ReturnNull_WhenSaleDoesNotExist()
        {
            // Arrange
            var query = new GetSaleQuery { Id = Guid.NewGuid() };
            _saleRepository.GetByIdAsync(query.Id).Returns((Sale)null!);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
