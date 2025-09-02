namespace Jobee.Pricing.Domain.ValueObjects;

public readonly record struct DateTimeRange
{
    public DateTimeRange(DateTimeOffset? startsAt, DateTimeOffset? endsAt)
    {
        if (startsAt > endsAt)
        {
            throw new ArgumentException("StartsAt cannot be later than EndsAt");
        }
        
        StartsAt = startsAt;
        EndsAt = endsAt;
    }
    
    public DateTimeOffset? StartsAt { get; private init; }
    public DateTimeOffset? EndsAt { get; private init; }
    
    public bool IsInfinite => !StartsAt.HasValue && !EndsAt.HasValue;
    
    public bool Overlaps(DateTimeRange other)
    {
        if (!StartsAt.HasValue && !EndsAt.HasValue)
        {
            return true;
        }

        if (!other.StartsAt.HasValue && !other.EndsAt.HasValue)
        {
            return true;
        }
 
        var startsAt = StartsAt ?? DateTimeOffset.MinValue;
        var endsAt = EndsAt ?? DateTimeOffset.MaxValue;
        
        var otherStartsAt = other.StartsAt ?? DateTimeOffset.MinValue;
        var otherEndsAt = other.EndsAt ?? DateTimeOffset.MaxValue;
        
        if (startsAt <= otherStartsAt)
        {
            return endsAt >= otherStartsAt;
        }

        return endsAt <= otherEndsAt;
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