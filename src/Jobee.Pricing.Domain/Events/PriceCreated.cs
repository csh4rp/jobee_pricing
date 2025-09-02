using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.Domain.Events;

public record PriceCreated(Guid Id, DateTimeRange DateTimeRange, decimal Amount);