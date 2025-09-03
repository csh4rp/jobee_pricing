using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Domain;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    
    Task UpdateAsync(Product product, CancellationToken cancellationToken);
    
    Task ArchiveAsync(Product product, CancellationToken cancellationToken);
    
    Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Product> GetByIdAsync(Guid id, DateTimeOffset timestamp, CancellationToken cancellationToken);
}