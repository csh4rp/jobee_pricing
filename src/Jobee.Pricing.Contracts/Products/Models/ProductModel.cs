namespace Jobee.Pricing.Contracts.Products.Models;

public record ProductModel
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }
    
    public required int NumberOfOffers { get; init; }
}