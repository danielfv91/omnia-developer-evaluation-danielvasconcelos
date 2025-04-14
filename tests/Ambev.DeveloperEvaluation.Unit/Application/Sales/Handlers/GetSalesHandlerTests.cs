using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Handlers
{

    public class GetSalesHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly GetSalesHandler _handler;

        public GetSalesHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Sale, SaleListItemDto>();
            });

            _mapper = config.CreateMapper();
            _handler = new GetSalesHandler(_saleRepository, _mapper);
        }

        [Fact]
        public async Task Handle_Should_ReturnPagedSalesSuccessfully()
        {
            // Arrange
            var query = new GetSalesQuery
            {
                Page = 1,
                Size = 10,
                Order = "SaleDate desc",
                Branch = "Test Branch"
            };

            var fakeSales = Enumerable.Range(0, 5)
                .Select(_ => SaleFakerBuilder.GenerateValidSaleWithItems())
                .ToList();

            _saleRepository
                .GetSalesPagedAsync(query.Page, query.Size, query.Order, query.Branch, query.MinDate, query.MaxDate)
                .Returns((fakeSales, fakeSales.Count));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CurrentPage.Should().Be(1);
            result.TotalItems.Should().Be(fakeSales.Count);
            result.TotalPages.Should().Be(1);
            result.Data.Should().HaveCount(fakeSales.Count);
        }
    }
}