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
        var product = await session.Query<ProductProjectionModel>()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Product), request.Id);

        return new ProductDetailsModel
        {
            Id = product.Id,
            Name = product.Name,
            Prices = [.. product.Prices.Select(p => new PriceModel
            {
                Id = p.Id,
                Amount = p.Money.Amount,
                Currency = p.Money.Currency,
                EndsAt = p.DateTimeRange.EndsAt,
                StartsAt = p.DateTimeRange.StartsAt
            })]
        };
    }
}