using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.ValueObjects;
using Jobee.Utils.Application.Exceptions;
using Marten;
using Marten.Services;

namespace Jobee.Pricing.Infrastructure.DataAccess;

public class ProductRepository : IProductRepository
{
    private readonly IDocumentStore _documentStore;

    public ProductRepository(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();
        _ = session.Events.StartStream<Product>(product.Id, product.DequeueEvents());

        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();
        _ = session.Events.Append(product.Id, product.DequeueEvents());

        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task ArchiveAsync(Product product, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();
        session.Events.ArchiveStream(product.Id);

        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.OpenSession(new SessionOptions());
        return await session.Events.AggregateStreamToLastKnownAsync<Product>(id, token: cancellationToken)
                      ?? throw new EntityNotFoundException(nameof(Product), id);
    }

    public async Task<Product> GetByVersionAsync(ProductVersion version, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();
        return await session.Events.AggregateStreamAsync<Product>(version.Id,
                   version: version.Number,
                   token: cancellationToken)
               ?? throw new EntityNotFoundException(nameof(ProductVersion), version);
    }
}