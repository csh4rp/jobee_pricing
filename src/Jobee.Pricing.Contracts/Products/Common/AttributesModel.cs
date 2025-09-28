namespace Jobee.Pricing.Contracts.Products.Common;

public record AttributesModel
{
    public required int NumberOfLocations { get; init; }
    
    public required int NumberOfBumps { get; init; }
    
    public required int DurationInDays { get; init; } 
}