namespace Jobee.Pricing.Contracts.Common;

public interface IPriceModel
{
    DateTimeOffset? StartsAt { get; }

    DateTimeOffset? EndsAt { get; }
    
    decimal Amount { get; }
}