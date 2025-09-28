namespace Jobee.Pricing.Domain.Products;

public record FeatureFlags
{
    public required bool HasPriority { get; init; }
}