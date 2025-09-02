using Jobee.Pricing.Contracts.Commands;
using Jobee.Pricing.Contracts.Creation;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.ValueObjects;
using Jobee.Utils.Contracts.Responses;
using Microsoft.Extensions.Logging;

namespace Jobee.Pricing.Application.Creation;

public class CreateProductCommandHandler
{
    public static async Task<CreatedResponse<Guid>> Handle(CreateProductCommand request,
        IProductRepository productRepository,
        ILogger<CreateProductCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            Guid.CreateVersion7(),
            request.Name,
            request.NumberOfOffers,
            request.IsActive,
            [.. request.Prices.Select(price => new Price(Guid.CreateVersion7(), new DateTimeRange(price.StartsAt, price.EndsAt), price.Amount))]);

        await productRepository.AddAsync(product, cancellationToken);
        
        logger.LogInformation("Product with id: {Id} and name: {Name} created", product.Id, product.Name);

        return new CreatedResponse<Guid>
        {
            Id = product.Id
        };
    }
}