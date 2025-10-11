using Jobee.Pricing.Contracts.Products.Models;
using Jobee.Pricing.Contracts.Products.Queries;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Infrastructure.Products.Projections;
using Jobee.Utils.Application.Exceptions;
using Marten;

namespace Jobee.Pricing.Infrastructure.Products.Handlers;

public class GetProductQueryHandler
{
    public static async Task<ProductDetailsModel> Handle(GetProductQuery request,
        IDocumentStore documentStore,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.LightweightSession();
        var product = await session.Query<ProductProjection>()
            .FirstOrDefaultAsync(p => p.ProductId == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Product), request.Id);

        return new ProductDetailsModel
        {
            Id = product.ProductId,
            Name = product.ProductName,
            Prices = [.. product.Prices.Select(p => new PriceModel
            {
                Id = p.Id,
                Amount = p.Value.Amount,
                Currency = p.Value.Currency,
                EndsAt = p.DateTimeRange.EndsAt,
                StartsAt = p.DateTimeRange.StartsAt
            })]
        };
    }
}