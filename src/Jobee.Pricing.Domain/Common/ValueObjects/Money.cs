namespace Jobee.Pricing.Domain.Common.ValueObjects;

public readonly record struct Money(decimal Amount, Currency Currency);