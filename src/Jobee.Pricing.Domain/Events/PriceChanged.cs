using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Domain.Events;

public record PriceChanged(Guid Id, DateRange DateRange, decimal Amount);