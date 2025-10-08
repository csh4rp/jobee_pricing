namespace Jobee.Pricing.Domain.Packages;

public interface IPackageRepository
{
    Task AddAsync(Package package, CancellationToken cancellationToken);
    
    Task UpdateAsync(Package package, CancellationToken cancellationToken);
    
    Task ArchiveAsync(Package package, CancellationToken cancellationToken);
    
    Task<Package> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<bool> ExistsForProductAsync(Guid productId, CancellationToken cancellationToken);
}