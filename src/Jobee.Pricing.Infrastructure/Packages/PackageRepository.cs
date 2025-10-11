using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Infrastructure.Packages.Projections;
using Jobee.Utils.Application.Exceptions;
using Marten;
using Marten.Services;

namespace Jobee.Pricing.Infrastructure.Packages;

public class PackageRepository : IPackageRepository
{
    private readonly IDocumentStore _documentStore;

    public PackageRepository(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task AddAsync(Package package, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();
        _ = session.Events.StartStream<Package>(package.Id, package.DequeueEvents());
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Package package, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();
        _ = session.Events.Append(package.Id, package.DequeueEvents());
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task ArchiveAsync(Package package, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();
        session.Events.Append(package.Id, new PackageArchived());
        session.Events.ArchiveStream(package.Id);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<Package> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession(new SessionOptions());
        return await session.Events.AggregateStreamToLastKnownAsync<Package>(id, token: cancellationToken)
               ?? throw new EntityNotFoundException(nameof(Package), id);
    }

    public async Task<bool> ExistsForProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession(new SessionOptions());
        return await session.DocumentStore.QuerySession().Query<PackageProjection>()
            .AnyAsync(p => p.ProductId == productId, cancellationToken);
    }
}