namespace Jobee.Pricing.Contracts.Models;

public record CreatePriceModel
{
    public required DateTimeOffset? StartsAt { get; init; }
    
    public required DateTimeOffset? EndsAt { get; init; }
    
    public required decimal Amount { get; init; }
}