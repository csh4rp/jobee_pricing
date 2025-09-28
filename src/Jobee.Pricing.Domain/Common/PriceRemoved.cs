namespace Jobee.Pricing.Domain.Common;

public record PriceRemoved(Guid Id, DateTimeRange DateTimeRange, Money Money);