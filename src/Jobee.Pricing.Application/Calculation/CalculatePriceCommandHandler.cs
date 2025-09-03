using Jobee.Pricing.Contracts.Commands;
using Jobee.Pricing.Contracts.Models;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Application.Calculation;

public class CalculatePriceCommandHandler
{
    public static async Task<PriceCalculationResult> Handle(CalculatePriceCommand request,
        IProductRepository productRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var product = request.PurchasedAt.HasValue 
            ? await productRepository.GetByIdAsync(request.ProductId, request.PurchasedAt.Value, cancellationToken)
            : await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        
        var price = product.GetPrice(timeProvider.GetUtcNow());

        return new PriceCalculationResult(price.Amount);
    }
}