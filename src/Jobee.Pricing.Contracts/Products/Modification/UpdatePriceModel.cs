using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Products.Modification;

public record UpdatePriceModel : IPriceModel
{
    public required Guid? Id { get; init; }
    
    public required DateTimeOffset? StartsAt { get; init; }
    
    public required DateTimeOffset? EndsAt { get; init; }
    
    public required decimal Amount { get; init; }
}