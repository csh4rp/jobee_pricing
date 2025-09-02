using Jobee.Pricing.Contracts.Common;

namespace Jobee.Pricing.Contracts.Modification;

public record UpdatePriceModel : IPriceModel
{
    public required Guid? Id { get; init; }
    
    public required DateTimeOffset? StartsAt { get; init; }
    
    public required DateTimeOffset? EndsAt { get; init; }
    
    public required decimal Amount { get; init; }
}