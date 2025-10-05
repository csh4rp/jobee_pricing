using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Products;

public record ProductPriceChanged(Guid Id, DateTimeRange DateTimeRange, Money Money);