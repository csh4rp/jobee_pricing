using Jobee.Pricing.Contracts.Models;

namespace Jobee.Pricing.Contracts.Commands;

public record CreateProductCommand
{
    public required string Name { get; init; }
    
    public required int NumberOfOffers { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required IReadOnlyList<CreatePriceModel> Prices { get; init; }
}