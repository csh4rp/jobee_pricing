using JasperFx.Events.Projections;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Jobee.Pricing.Infrastructure.Products;
using Jobee.Pricing.Infrastructure.Products.Projections;
using Jobee.Pricing.Infrastructure.Settings;
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
        services.AddTransient<ISettingRepository, SettingRepository>();
        services.AddTransient<CurrencyConverter>();
        services.AddTransient<SettingsService>();
        
        return services;
    }
}