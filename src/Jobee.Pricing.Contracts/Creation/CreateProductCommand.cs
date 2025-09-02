namespace Jobee.Pricing.Contracts.Creation;

public record CreateProductCommand
{
    public required string Name { get; init; }
    
    public required int NumberOfOffers { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required IReadOnlyList<CreatePriceModel> Prices { get; init; }
}