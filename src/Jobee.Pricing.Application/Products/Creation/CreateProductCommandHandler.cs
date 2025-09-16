using Jobee.Pricing.Contracts.Products.Creation;
using Jobee.Pricing.Domain;
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
            new Price(Guid.CreateVersion7(),
            new DateTimeRange(price.StartsAt, price.EndsAt), 
            new Money(price.Amount, defaultCurrency))
        ).ToList();
        
        var product = new Product(
            Guid.CreateVersion7(),
            request.Name,
            request.NumberOfOffers,
            request.IsActive,
            prices);

        await productRepository.AddAsync(product, cancellationToken);
        
        logger.LogInformation("Product with id: {Id} and name: {Name} created", product.Id, product.Name);

        return new CreatedResponse<Guid>
        {
            Id = product.Id
        };
    }
}