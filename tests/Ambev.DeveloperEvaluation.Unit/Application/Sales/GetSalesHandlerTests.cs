using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

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
        var sales = GenerateFakeSales(5);
        _saleRepository.GetSalesPagedAsync(1, 10, "SaleDate desc", null, null, null)
            .Returns((sales, sales.Count));

        var query = new GetSalesQuery
        {
            Page = 1,
            Size = 10,
            Order = "SaleDate desc"
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sales.Count, result.TotalItems);
        Assert.Equal(sales.Count, result.Data.Count());
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(1, result.TotalPages);
    }

    private List<Sale> GenerateFakeSales(int count)
    {
        var faker = new Faker("pt_BR");
        var sales = new List<Sale>();

        for (int i = 0; i < count; i++)
        {
            var saleId = Guid.NewGuid();
            sales.Add(new Sale
            {
                Id = saleId,
                SaleNumber = faker.Random.Int(1000, 9999),
                SaleDate = faker.Date.Recent(),
                CustomerId = Guid.NewGuid(),
                CustomerName = faker.Name.FullName(),
                Branch = faker.Company.CompanyName(),
                TotalAmount = faker.Random.Decimal(100, 1000),
                IsCancelled = false,
                Items = new List<SaleItem>()
            });
        }

        return sales;
    }
}
