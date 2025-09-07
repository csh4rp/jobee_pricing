using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Domain.Entities;

public record Price
{
    public Guid Id { get; init; }

    public DateTimeRange DateTimeRange { get; init; }

    public decimal Amount { get; init; }

    public bool IsDefault => DateTimeRange.StartsAt == null && DateTimeRange.EndsAt == null;

    public Price(Guid id, DateTimeRange dateTimeRange, decimal amount)
    {
        Id = id;
        DateTimeRange = dateTimeRange;
        Amount = amount;
    }

    public static Price Default(decimal amount) => new(Guid.CreateVersion7(), new DateTimeRange(null, null), amount);
}