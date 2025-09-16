namespace Jobee.Pricing.Contracts.Products.Common;

public interface IPriceModel
{
    DateTimeOffset? StartsAt { get; }

    DateTimeOffset? EndsAt { get; }
    
    decimal Amount { get; }
}