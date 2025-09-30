using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Common;

public record CreatePriceModel : IPriceModel
{
    public required DateTimeOffset? StartsAt { get; init; }
    
    public required DateTimeOffset? EndsAt { get; init; }
    
    public required decimal Amount { get; init; }
}