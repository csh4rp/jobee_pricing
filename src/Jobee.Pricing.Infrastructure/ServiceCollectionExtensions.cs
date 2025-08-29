using JasperFx.Events.Projections;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Infrastructure.DataAccess;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Marten;

namespace Jobee.Pricing.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMarten(o =>
            {
                o.Connection(configuration.GetConnectionString("Database")!);
                o.Projections.Add<ProductProjection>(ProjectionLifecycle.Inline);
            })
            .IntegrateWithWolverine()
            .ApplyAllDatabaseChangesOnStartup();
        
        services.AddTransient<IProductRepository, ProductRepository>();
        
        return services;
    }
}