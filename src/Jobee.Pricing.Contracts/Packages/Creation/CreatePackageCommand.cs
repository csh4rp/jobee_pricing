using Jobee.Pricing.Contracts.Common;

namespace Jobee.Pricing.Contracts.Packages.Creation;

public record CreatePackageCommand
{
    public required Guid ProductId { get; init; }
    
    public required int Quantity { get; init; }
    
    public required string Name { get; init; }

    public required string Description { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required IReadOnlyList<CreatePriceModel> Prices { get; init; }
}