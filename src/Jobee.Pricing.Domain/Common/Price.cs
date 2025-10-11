namespace Jobee.Pricing.Domain.Common;

public record Price
{
    public Guid Id { get; init; }

    public DateTimeRange DateTimeRange { get; init; }

    public Money Value { get; init; }

    public bool IsDefault => DateTimeRange.StartsAt == null && DateTimeRange.EndsAt == null;

    public Price(Guid id, DateTimeRange dateTimeRange, Money value)
    {
        Id = id;
        DateTimeRange = dateTimeRange;
        Value = value;
        
    }
    
    public Price(DateTimeRange dateTimeRange, Money value) : this(Guid.CreateVersion7(), dateTimeRange, value)
    {
    }
}