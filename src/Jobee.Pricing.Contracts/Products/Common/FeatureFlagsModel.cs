namespace Jobee.Pricing.Contracts.Products.Common;

public record FeatureFlagsModel
{
    public required bool HasPriority { get; init; }
}