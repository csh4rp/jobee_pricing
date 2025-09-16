using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.Domain.Products;

public record PriceRemoved(Guid Id, DateTimeRange DateTimeRange, Money Money);