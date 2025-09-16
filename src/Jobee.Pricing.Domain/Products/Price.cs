using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.Domain.Products;

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
}