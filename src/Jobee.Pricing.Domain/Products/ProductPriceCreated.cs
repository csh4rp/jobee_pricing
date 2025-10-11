using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Products;

public record ProductPriceCreated(Guid Id, DateTimeRange DateTimeRange, Money Price);