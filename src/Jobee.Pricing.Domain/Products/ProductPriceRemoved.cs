using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Products;

public record ProductPriceRemoved(Guid Id, DateTimeRange DateTimeRange, Money Money);