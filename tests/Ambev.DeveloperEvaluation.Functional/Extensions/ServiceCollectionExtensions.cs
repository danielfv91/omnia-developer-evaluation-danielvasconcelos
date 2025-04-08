using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Functional.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<TContext>)
        );

        if (descriptor != null)
            services.Remove(descriptor);
    }
}
