using Jobee.Pricing.Contracts.Commands;
using Jobee.Pricing.Contracts.Models;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Application.Calculation;

public class CalculatePriceCommandHandler
{
    public async Task<PriceCalculationResult> Handle(CalculatePriceCommand request,
        IProductRepository productRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var productVersion = new ProductVersion(request.ProductVersion);
        
        var product = await productRepository.GetByVersionAsync(productVersion, cancellationToken);
        
        var price = product.GetPrice(timeProvider.GetUtcNow());

        return new PriceCalculationResult(price.Amount);
    }
}