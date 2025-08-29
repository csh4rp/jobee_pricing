using Jobee.Pricing.Contracts.Commands;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Jobee.Pricing.Application.Handlers;

public class UpdateProductCommandHandler
{
    public static async Task Handle(UpdateProductCommand request,
        IProductRepository productRepository,
        ILogger<UpdateProductCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        
        product.Update(
            request.Name,
            request.NumberOfOffers,
            request.IsActive,
            [.. request.Prices.Select(price => new Price(Guid.CreateVersion7(), new DateRange(price.StartsAt, price.EndsAt), price.Amount))]);

        await productRepository.UpdateAsync(product, cancellationToken);
        
        logger.LogInformation("Product with id: {Id} and name: {Name} updated", product.Id, product.Name);
    }
}