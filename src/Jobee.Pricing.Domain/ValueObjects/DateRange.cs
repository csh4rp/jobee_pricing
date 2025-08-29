namespace Jobee.Pricing.Domain.ValueObjects;

public readonly record struct DateRange(DateTimeOffset? StartsAt, DateTimeOffset? EndsAt)
{
    public bool Overlaps(DateRange other)
    {
        if (!StartsAt.HasValue && !EndsAt.HasValue)
        {
            return true;
        }
        
        if (!StartsAt.HasValue)
        {
            return other.EndsAt.HasValue && other.EndsAt.Value >= EndsAt;
        }
        
        return false;
    }
    
    public bool Overlaps(DateTimeOffset timestamp)
    {
        if (!StartsAt.HasValue && !EndsAt.HasValue)
        {
            return true;
        }
        
        if (!StartsAt.HasValue)
        {
            return timestamp <= EndsAt.GetValueOrDefault();
        }
        
        if (!EndsAt.HasValue)
        {
            return timestamp >= StartsAt.Value;
        }
        
        return timestamp >= StartsAt.Value && timestamp <= EndsAt.Value;
    }
}