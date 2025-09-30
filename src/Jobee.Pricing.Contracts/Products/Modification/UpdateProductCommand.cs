using Jobee.Pricing.Contracts.Common;
using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Products.Modification;

public record UpdateProductCommand
{
    public Guid ProductId { get; private set; }

    public required string Name { get; init; }

    public required string Description { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required FeatureFlagsModel FeatureFlags { get; init; }
    
    public required AttributesModel Attributes { get; init; }
    
    public required IReadOnlyList<UpdatePriceModel> Prices { get; init; }
    
    public void SetProductId(Guid id) => ProductId = id;
}