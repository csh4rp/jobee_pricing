namespace Jobee.Pricing.Infrastructure.DataAccess.Models;

public record PriceProjectionModel
{
    public Guid Id { get; init; }
    
    public DateTimeOffset? StartsAt { get; init; } 
    
    public DateTimeOffset? EndsAt { get; init; }
    
    public decimal Amount { get; init; }

}