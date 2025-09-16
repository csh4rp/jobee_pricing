using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.Domain.Products;

public record PriceCreated(Guid Id, DateTimeRange DateTimeRange, Money Money);