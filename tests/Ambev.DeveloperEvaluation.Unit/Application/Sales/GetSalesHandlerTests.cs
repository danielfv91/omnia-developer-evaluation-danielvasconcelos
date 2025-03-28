using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Builders;
using AutoMapper;
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
        var branch = "Branch Test";
        var fakeSales = Enumerable.Range(0, 5)
            .Select(_ => SaleFakerBuilder.GenerateValidSaleWithItems())
            .ToList();

        _saleRepository.GetSalesPagedAsync(1, 10, "SaleDate desc", branch, null, null)
            .Returns((fakeSales, fakeSales.Count));

        var query = new GetSalesQuery
        {
            Page = 1,
            Size = 10,
            Order = "SaleDate desc",
            Branch = branch
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fakeSales.Count, result.TotalItems);
        Assert.Equal(fakeSales.Count, result.Data.Count());
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(1, result.TotalPages);
    }
}
