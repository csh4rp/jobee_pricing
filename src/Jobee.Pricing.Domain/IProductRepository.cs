using Jobee.Pricing.Domain.Entities;

namespace Jobee.Pricing.Domain;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    
    Task UpdateAsync(Product product, CancellationToken cancellationToken);
    
    Task ArchiveAsync(Product product, CancellationToken cancellationToken);
    
    Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Product> GetByIdAsync(Guid id, long version, CancellationToken cancellationToken);
}