using Jobee.Pricing.Contracts.Models;
using Jobee.Pricing.Contracts.PriceCalculation;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.ValueObjects;
using Jobee.Utils.Application.Exceptions;
using Jobee.Utils.Contracts;

namespace Jobee.Pricing.Application.Calculation;

public class CalculatePriceCommandHandler
{
    public static async Task<PriceCalculationResult> Handle(CalculatePriceCommand request,
        IProductRepository productRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        bool hasVersion = false;
        long version = 0;
        Guid productId;
        
        if (ProductVersion.TryParse(request.ProductId, out var productVersion))
        {
            (productId, version) = productVersion;
            hasVersion = true;
        }
        else if (Guid.TryParse(request.ProductId, out var guid))
        {
            productId = guid;
        }
        else
        {
            throw new ValidationException("Invalid ProductId format", [MemberError.InvalidValue(nameof(request.ProductId), [])]);
        }
        
        var product = hasVersion
            ? await productRepository.GetByIdAsync(productId, version, cancellationToken)
            : await productRepository.GetByIdAsync(productId, cancellationToken);
        
        var price = product.GetPrice(timeProvider.GetUtcNow());

        return new PriceCalculationResult(price.Amount);
    }
}