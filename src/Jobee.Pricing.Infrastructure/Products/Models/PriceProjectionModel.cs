using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.Infrastructure.Products.Models;

public record PriceProjectionModel
{
    public Guid Id { get; init; }
    
    public DateTimeOffset? StartsAt { get; init; } 
    
    public DateTimeOffset? EndsAt { get; init; }
    
    public Money Money { get; init; }

}