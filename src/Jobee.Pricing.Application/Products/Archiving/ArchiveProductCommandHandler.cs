using Jobee.Pricing.Contracts.Products.Archiving;
using Jobee.Pricing.Domain.Products;
using Microsoft.Extensions.Logging;
using Wolverine.Attributes;

namespace Jobee.Pricing.Application.Products.Archiving;

public class ArchiveProductCommandHandler
{
    [Transactional]
    public static async Task Handle(ArchiveProductCommand request,
        IProductRepository productRepository,
        ILogger<ArchiveProductCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        await productRepository.ArchiveAsync(product, cancellationToken);
        
        logger.LogInformation("Product with id: {id} and name: {name} archived", product.Id, product.Name);
    }
}