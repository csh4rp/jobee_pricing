namespace Jobee.Pricing.Contracts.Products.Models;

public record ProductPriceModel
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }
    
    public required string Description { get; init; }
    
    public required decimal Price { get; init; }
}