using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using FluentValidation;
using System.Globalization;

namespace Ambev.DeveloperEvaluation.Integration.Common;

public class IntegrationTestFixture : IAsyncLifetime
{
    private PostgreSqlContainer _dbContainer = default!;
    public IServiceProvider ServiceProvider = default!;

    public async Task InitializeAsync()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

        var containerName = $"integration-tests-db-{Guid.NewGuid()}";

        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithCleanUp(true)
            .WithName(containerName)
            .WithDatabase("developer_integration_evaluation_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(0, 5432)
            .WithEnvironment("TZ", "UTC")
            .Build();

        await _dbContainer.StartAsync();

        var services = new ServiceCollection();

        services.AddDbContext<DefaultContext>(options =>
        {
            options.UseNpgsql(_dbContainer.GetConnectionString());
        });

        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISaleItemBuilder, SaleItemBuilder>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<ISaleItemCalculator, SaleItemCalculator>();

        services.AddAutoMapper(typeof(CreateSaleCommand).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateSaleCommand>();
        });

        services.AddValidatorsFromAssemblyContaining<CreateSaleValidator>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        ServiceProvider = services.BuildServiceProvider();

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        await context.Database.MigrateAsync();

        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Sales\" CASCADE;");
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
