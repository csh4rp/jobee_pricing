using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.Contracts.Products.Models;

public record PriceModel
{
    public required Guid Id { get; init; }
    
    public required DateTimeOffset? StartsAt { get; init; }
    
    public required DateTimeOffset? EndsAt { get; init; }
    
    public required decimal Amount { get; init; }
    
    public required Currency Currency { get; init; }
}