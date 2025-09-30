using Jobee.Pricing.Contracts.Common;
using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Products.Creation;

public record CreateProductCommand
{
    public required string Name { get; init; }

    public required string Description { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required FeatureFlagsModel FeatureFlags { get; init; }
    
    public required AttributesModel Attributes { get; init; }
    
    public required IReadOnlyList<CreatePriceModel> Prices { get; init; }
}