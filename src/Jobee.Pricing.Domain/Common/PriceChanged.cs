namespace Jobee.Pricing.Domain.Common;

public record PriceChanged(Guid Id, DateTimeRange DateTimeRange, Money Money);