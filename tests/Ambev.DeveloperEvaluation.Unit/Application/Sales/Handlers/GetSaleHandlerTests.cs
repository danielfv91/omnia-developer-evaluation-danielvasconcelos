using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Handlers
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
        public async Task Handle_Should_ReturnMappedSale_WhenSaleExists()
        {
            // Arrange
            var fakeSale = SaleFakerBuilder.GenerateValidSaleWithItems(2);
            _saleRepository.GetByIdAsync(fakeSale.Id).Returns(fakeSale);

            var query = new GetSaleQuery { Id = fakeSale.Id };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.SaleNumber.Should().Be(fakeSale.SaleNumber);
            result.CustomerName.Should().Be(fakeSale.CustomerName);
            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_Should_ThrowNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            var query = new GetSaleQuery { Id = Guid.NewGuid() };
            _saleRepository.GetByIdAsync(query.Id).Returns((Sale?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Sale with ID {query.Id} was not found.");
        }
    }
}