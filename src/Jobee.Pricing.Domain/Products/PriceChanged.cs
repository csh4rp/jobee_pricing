using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.Domain.Products;

public record PriceChanged(Guid Id, DateTimeRange DateTimeRange, Money Money);