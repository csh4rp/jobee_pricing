namespace Jobee.Pricing.Domain.Products;

public record ProductAttributesChanged
{
    public int NumberOfLocations { get; init; }
    
    public int NumberOfBumps { get; init; }
    
    public TimeSpan Duration { get; init; } 
    
    private ProductAttributesChanged()
    {
    }
    
    public ProductAttributesChanged(Attributes attributes)
    {
        NumberOfLocations = attributes.NumberOfLocations;
        NumberOfBumps = attributes.NumberOfBumps;
        Duration = attributes.Duration;
    }
}