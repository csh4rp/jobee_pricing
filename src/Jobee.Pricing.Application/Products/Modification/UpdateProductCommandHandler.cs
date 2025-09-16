using Jobee.Pricing.Contracts.Products.Modification;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Microsoft.Extensions.Logging;

namespace Jobee.Pricing.Application.Products.Modification;

public class UpdateProductCommandHandler
{
    public static async Task Handle(UpdateProductCommand request,
        IProductRepository productRepository,
        SettingsService settingsService,
        ILogger<UpdateProductCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var defaultCurrency = await settingsService.GetDefaultCurrencyAsync(cancellationToken);
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        
        var prices = request.Prices.Select(price => 
            new Price(price.Id ?? Guid.CreateVersion7(),
            new DateTimeRange(price.StartsAt, price.EndsAt), 
            new Money(price.Amount, defaultCurrency))
        ).ToList();
        
        product.Update(
            request.Name,
            request.NumberOfOffers,
            request.IsActive,
            prices);

        await productRepository.UpdateAsync(product, cancellationToken);
        
        logger.LogInformation("Product with id: {Id} and name: {Name} updated", product.Id, product.Name);
    }
}