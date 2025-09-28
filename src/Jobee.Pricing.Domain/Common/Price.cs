namespace Jobee.Pricing.Domain.Common;

public record Price
{
    public Guid Id { get; init; }

    public DateTimeRange DateTimeRange { get; init; }

    public Money Money { get; init; }

    public bool IsDefault => DateTimeRange.StartsAt == null && DateTimeRange.EndsAt == null;

    public Price(Guid id, DateTimeRange dateTimeRange, Money money)
    {
        Id = id;
        DateTimeRange = dateTimeRange;
        Money = money;
        
    }
    
    public Price(DateTimeRange dateTimeRange, Money money) : this(Guid.CreateVersion7(), dateTimeRange, money)
    {
    }
}