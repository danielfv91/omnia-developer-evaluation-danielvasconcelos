using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
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
            var sale = GenerateFakeSale();
            _saleRepository.GetByIdAsync(sale.Id).Returns(sale);

            var query = new GetSaleQuery { Id = sale.Id };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sale.SaleNumber, result.SaleNumber);
            Assert.Equal(sale.CustomerName, result.CustomerName);
            Assert.Equal(sale.Items.Count, result.Items.Count);
        }

        [Fact]
        public async Task Handle_Should_ReturnNull_WhenSaleDoesNotExist()
        {
            // Arrange
            var query = new GetSaleQuery { Id = Guid.NewGuid() };
            _saleRepository.GetByIdAsync(query.Id).Returns((Sale)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        private Sale GenerateFakeSale()
        {
            var faker = new Faker("pt_BR");
            var saleId = Guid.NewGuid();

            return new Sale
            {
                Id = saleId,
                SaleNumber = faker.Random.Int(1000, 9999),
                SaleDate = faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Name.FullName(),
                Branch = faker.Company.CompanyName(),
                TotalAmount = faker.Random.Decimal(100, 1000),
                IsCancelled = false,
                Items = new List<SaleItem>
                {
                    new SaleItem
                    {
                        Id = Guid.NewGuid(),
                        SaleId = saleId,
                        ProductId = Guid.NewGuid(),
                        ProductName = faker.Commerce.ProductName(),
                        Quantity = 3,
                        UnitPrice = 50,
                        TotalItemAmount = 150,
                        IsCancelled = false
                    }
                }
            };
        }
    }
}