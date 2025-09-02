using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Domain.Events;

public record PriceRemoved(Guid Id, DateTimeRange DateTimeRange, decimal Amount);