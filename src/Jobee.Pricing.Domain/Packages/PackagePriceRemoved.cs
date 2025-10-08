using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Packages;

public record PackagePriceRemoved(Guid Id, DateTimeRange DateTimeRange, Money Money);