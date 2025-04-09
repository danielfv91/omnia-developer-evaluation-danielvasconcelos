﻿using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Services.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddScoped<ISaleRepository, SaleRepository>();

        builder.Services.AddScoped<IEventPublisher, EventPublisher>();

        builder.Services.AddScoped<ISaleItemCalculator, SaleItemCalculator>();

        builder.Services.AddScoped<ISaleItemBuilder, SaleItemBuilder>();

    }
}