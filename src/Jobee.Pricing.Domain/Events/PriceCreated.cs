using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Domain.Events;

public record PriceCreated(Guid Id, DateRange DateRange, decimal Amount);