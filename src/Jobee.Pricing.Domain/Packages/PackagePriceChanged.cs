using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Packages;

public record PackagePriceChanged(Guid Id, DateTimeRange DateTimeRange, Money Value);