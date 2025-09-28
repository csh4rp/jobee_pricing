namespace Jobee.Pricing.Domain.Common;

public record PriceCreated(Guid Id, DateTimeRange DateTimeRange, Money Money);