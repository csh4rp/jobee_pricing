using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Domain;

public record Price
{
    public Guid Id { get; init; }
    
    public DateRange DateRange { get; init; }
    
    public decimal Amount { get; init; }

    public bool IsDefault => DateRange.StartsAt == null && DateRange.EndsAt == null;
    
    public Price(Guid id, DateRange dateRange, decimal amount)
    {
        Id = id;
        DateRange = dateRange;
        Amount = amount;
    }
    
    public static Price Default(decimal amount) => new(Guid.CreateVersion7(), new  DateRange(null, null), amount);
}