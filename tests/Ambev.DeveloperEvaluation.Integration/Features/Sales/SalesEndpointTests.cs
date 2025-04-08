using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.Integration.Containers;
using Ambev.DeveloperEvaluation.Integration.Factory;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Xunit;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Integration.Features.Sales;

public class SalesEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainerFactory _containerFactory;
    private CustomWebApplicationFactory _factory;
    private HttpClient _client;
    private IMapper _mapper;
    private string _connectionString;

    public SalesEndpointTests()
    {
        _containerFactory = new PostgreSqlContainerFactory();
    }

    public async Task InitializeAsync()
    {
        _connectionString = await _containerFactory.StartAsync();
        _factory = new CustomWebApplicationFactory(_connectionString);
        _client = _factory.CreateClient();

        _mapper = _factory.Services.GetRequiredService<IMapper>();
    }

    public async Task DisposeAsync()
    {
        await _containerFactory.DisposeAsync();
    }

    [Fact]
    public async Task GetSales_Should_Return200Ok()
    {
        // Act
        var response = await _client.GetAsync("/api/sales?page=1&size=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateSale_Should_Return201Created_And_Fetch_By_Id()
    {
        // Arrange
        var faker = new Faker();
        var request = new CreateSaleRequest
        {
            SaleNumber = faker.Random.Int(1000, 9999),
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Name.FullName(),
            Branch = faker.Company.CompanyName(),
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 3,
                    UnitPrice = 10
                }
            }
        };

        // Act - Create
        var postResponse = await _client.PostAsJsonAsync("/api/sales", request);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        created.Should().NotBeNull();
        created!.Success.Should().BeTrue();

        var id = created.Data!.Id;

        // Act - Get by ID
        var getResponse = await _client.GetAsync($"/api/sales/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var sale = await getResponse.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();
        sale!.Data!.SaleNumber.Should().Be(request.SaleNumber);
    }

    [Fact]
    public async Task UpdateSale_Should_Modify_And_Return_Updated_Data()
    {
        // Arrange - create first
        var faker = new Faker();
        var create = new CreateSaleRequest
        {
            SaleNumber = faker.Random.Int(1000, 9999),
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Name.FullName(),
            Branch = faker.Company.CompanyName(),
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 3,
                    UnitPrice = 10
                }
            }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/sales", create);
        var created = await postResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var id = created!.Data!.Id;

        // Act - update
        var update = new UpdateSaleRequest
        {
            Id = id,
            SaleNumber = create.SaleNumber + 100,
            SaleDate = DateTime.UtcNow,
            CustomerId = create.CustomerId,
            CustomerName = create.CustomerName + " Updated",
            Branch = create.Branch,
            Items = create.Items.Select(i => new UpdateSaleItemRequest
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        var putResponse = await _client.PutAsJsonAsync($"/api/sales/{id}", update);
        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await putResponse.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateSaleResponse>>();
        updated!.Success.Should().BeTrue();
        updated.Data!.CustomerName.Should().Contain("Updated");
    }

    [Fact]
    public async Task DeleteSale_Should_Remove_Sale()
    {
        // Arrange - create
        var faker = new Faker();
        var request = new CreateSaleRequest
        {
            SaleNumber = faker.Random.Int(1000, 9999),
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Name.FullName(),
            Branch = faker.Company.CompanyName(),
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = faker.Commerce.ProductName(),
                    Quantity = 2,
                    UnitPrice = 10
                }
            }
        };

        var post = await _client.PostAsJsonAsync("/api/sales", request);
        var created = await post.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var id = created!.Data!.Id;

        // Act - delete
        var delete = await _client.DeleteAsync($"/api/sales/{id}");
        delete.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - get again
        var get = await _client.GetAsync($"/api/sales/{id}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateSale_Should_ReturnBadRequest_When_Id_Mismatch()
    {
        // Arrange
        var faker = new Faker();
        var idFromUrl = Guid.NewGuid();
        var request = new UpdateSaleRequest
        {
            Id = Guid.NewGuid(),
            SaleNumber = faker.Random.Int(1000, 9999),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Name.FullName(),
            Branch = faker.Company.CompanyName(),
            Items = new List<UpdateSaleItemRequest>()
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/sales/{idFromUrl}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSaleById_Should_ReturnNotFound_When_Sale_Does_Not_Exist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/sales/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body.Should().NotBeNull();
        body!.Success.Should().BeFalse();
        body.Message.Should().Be("Sale not found");
    }

    [Fact]
    public async Task DeleteSale_Should_ReturnNotFound_When_Sale_Does_Not_Exist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/sales/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body.Should().NotBeNull();
        body!.Success.Should().BeFalse();
        body.Message.Should().Be("Sale not found.");
    }

}
