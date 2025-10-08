using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Packages;

public record PackagePriceCreated(Guid Id, DateTimeRange DateTimeRange, Money Money);