using Jobee.Pricing.Contracts.Models;
using Jobee.Pricing.Contracts.Queries;
using Jobee.Pricing.Domain.Entities;
using Jobee.Pricing.Infrastructure.DataAccess.Models;
using Jobee.Utils.Application.Exceptions;
using Marten;

namespace Jobee.Pricing.Infrastructure.DataAccess.Handlers;

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
            NumberOfOffers = product.NumberOfOffers,
            Prices = [.. product.Prices.Select(p => new PriceModel
            {
                Id = p.Id,
                Amount = p.Amount,
                EndsAt = p.EndsAt,
                StartsAt = p.StartsAt
            })]
        };
    }
}