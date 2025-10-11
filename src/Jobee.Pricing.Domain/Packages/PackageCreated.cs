using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Packages;

public record PackageCreated
{
    public required Guid Id { get; init; }
    
    public required Guid ProductId { get; init; }
    
    public required string Name { get; init; }
    
    public required string Description { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required int Quantity { get; init; }
    
    public required IReadOnlyList<Price> Prices { get; init; }
}