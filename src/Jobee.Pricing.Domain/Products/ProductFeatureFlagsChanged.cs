namespace Jobee.Pricing.Domain.Products;

public record ProductFeatureFlagsChanged
{
    public bool HasPriority { get; private init; }
    
    private ProductFeatureFlagsChanged()
    {
    }

    public ProductFeatureFlagsChanged(FeatureFlags featureFlags)
    {
        HasPriority = featureFlags.HasPriority;
    }
}