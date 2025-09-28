using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Products;

public record ProductCreated
{
    public required Guid ProductId { get; init; }
    
    public required string Name { get; init; }
    
    public required string Description { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required IReadOnlyList<Price> Prices { get; init; }
    
    public required FeatureFlags FeatureFlags { get; init; }
    
    public required Attributes Attributes { get; init; }
}