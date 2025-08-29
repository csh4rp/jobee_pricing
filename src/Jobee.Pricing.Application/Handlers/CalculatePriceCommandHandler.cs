using Jobee.Pricing.Contracts.Commands;
using Jobee.Pricing.Contracts.Models;
using Jobee.Pricing.Domain;
using Microsoft.Extensions.Logging;

namespace Jobee.Pricing.Application.Handlers;

public class CalculatePriceCommandHandler
{
    public async Task<PriceCalculationResult> Handle(CalculatePriceCommand request,
        IProductRepository productRepository,
        ILogger<CalculatePriceCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, request.Timestamp, cancellationToken);
        
        var price = product.GetPrice(request.Timestamp);

        return new PriceCalculationResult(price);
    }
}