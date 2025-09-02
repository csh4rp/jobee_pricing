using Jobee.Pricing.Contracts.Common;

namespace Jobee.Pricing.Contracts.Creation;

public record CreatePriceModel : IPriceModel
{
    public required DateTimeOffset? StartsAt { get; init; }
    
    public required DateTimeOffset? EndsAt { get; init; }
    
    public required decimal Amount { get; init; }
}