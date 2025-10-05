namespace Jobee.Pricing.Domain.Products;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    
    Task UpdateAsync(Product product, CancellationToken cancellationToken);
    
    Task ArchiveAsync(Product product, CancellationToken cancellationToken);
    
    Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}