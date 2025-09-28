using Jobee.Pricing.Contracts.Products.Creation;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Jobee.Utils.Contracts.Responses;
using Microsoft.Extensions.Logging;

namespace Jobee.Pricing.Application.Products.Creation;

public class CreateProductCommandHandler
{
    public static async Task<CreatedResponse<Guid>> Handle(CreateProductCommand request,
        IProductRepository productRepository,
        SettingsService settingsService,
        ILogger<CreateProductCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var defaultCurrency = await settingsService.GetDefaultCurrencyAsync(cancellationToken);

        var prices = request.Prices.Select(price => 
            new Price(new DateTimeRange(price.StartsAt, price.EndsAt), 
            new Money(price.Amount, defaultCurrency))
        ).ToList();
        
        var product = new Product(
            request.Name,
            request.Description,
            request.IsActive,
            new FeatureFlags
            {
                HasPriority = request.FeatureFlags.HasPriority,
            },
            new Attributes
            {
                Duration = TimeSpan.FromDays(request.Attributes.DurationInDays),
                NumberOfBumps = request.Attributes.NumberOfBumps,
                NumberOfLocations = request.Attributes.NumberOfLocations,
            },
            prices);

        await productRepository.AddAsync(product, cancellationToken);
        
        logger.LogInformation("Product with id: {id} and name: {name} created", product.Id, product.Name);

        return new CreatedResponse<Guid>
        {
            Id = product.Id
        };
    }
}