using Jobee.Pricing.Contracts.Products.Archiving;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Jobee.Utils.Application.Exceptions;
using Jobee.Utils.Contracts;
using Microsoft.Extensions.Logging;
using Wolverine.Attributes;

namespace Jobee.Pricing.Application.Products.Archiving;

public class ArchiveProductCommandHandler
{
    [Transactional]
    public static async Task Handle(ArchiveProductCommand command,
        IProductRepository productRepository,
        IPackageRepository packageRepository,
        ILogger<ArchiveProductCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.Id, cancellationToken);

        if (await packageRepository.ExistsForProductAsync(command.Id, cancellationToken))
        {
            throw new ValidationException("Cannot archive product with associated packages", 
                [MemberError.InvalidValue(nameof(command.Id), [])]);
        }
        
        await productRepository.ArchiveAsync(product, cancellationToken);
        
        logger.LogInformation("Product with id: {id} and name: {name} archived", product.Id, product.Name);
    }
}