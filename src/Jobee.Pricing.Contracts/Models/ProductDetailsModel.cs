namespace Jobee.Pricing.Contracts.Models;

public record ProductDetailsModel
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }
    
    public required int NumberOfOffers { get; init; }
    
    public required IReadOnlyList<PriceModel> Prices { get; init; }
}