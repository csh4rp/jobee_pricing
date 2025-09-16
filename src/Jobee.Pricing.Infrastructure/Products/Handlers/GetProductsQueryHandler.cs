using Jobee.Pricing.Contracts.Products.Models;
using Jobee.Pricing.Contracts.Products.Queries;
using Jobee.Pricing.Infrastructure.Products.Models;
using Jobee.Utils.Contracts;
using Marten;

namespace Jobee.Pricing.Infrastructure.DataAccess.Handlers;

public class GetProductsQueryHandler
{
    public static async Task<PaginatedResponse<ProductModel>> Handle(GetProductsQuery request,
        IDocumentStore documentStore,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.LightweightSession();
        var queryable = session.Query<ProductProjectionModel>().AsQueryable();

        if (!string.IsNullOrEmpty(request.Name))
        {
            queryable = queryable.Where(x => x.Name.StartsWith(request.Name));
        }
        
        if (request.IsActive.HasValue)
        {
            queryable = request.IsActive.Value
                ? queryable.Where(x => x.IsActive)
                : queryable.Where(x => !x.IsActive);
        }
        
        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .OrderBy(x => x.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ProductModel
            {
                Id = x.Id,
                Name = x.Name,
                NumberOfOffers = x.NumberOfOffers,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<ProductModel>
        {
            Items = items,
            TotalCount = totalCount,
        };
    }
}