using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.Domain.Common;

public readonly record struct Money(decimal Amount, Currency Currency);