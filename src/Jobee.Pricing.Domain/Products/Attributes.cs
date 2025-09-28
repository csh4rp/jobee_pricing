namespace Jobee.Pricing.Domain.Products;

public record Attributes
{
    public required int NumberOfLocations { get; init; }
    
    public required int NumberOfBumps { get; init; }
    
    public required TimeSpan Duration { get; init; } 
}