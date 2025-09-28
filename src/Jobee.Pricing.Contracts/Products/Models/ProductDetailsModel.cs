namespace Jobee.Pricing.Contracts.Products.Models;

public record ProductDetailsModel
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }
    
    public required IReadOnlyList<PriceModel> Prices { get; init; }
}