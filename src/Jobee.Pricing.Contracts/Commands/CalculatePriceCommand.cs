namespace Jobee.Pricing.Contracts.Commands;

public record CalculatePriceCommand
{
    public Guid ProductId { get; private set; }
    
    public required DateTimeOffset Timestamp { get; init; }
    
    public void SetProductId(Guid productId) => ProductId = productId;
}