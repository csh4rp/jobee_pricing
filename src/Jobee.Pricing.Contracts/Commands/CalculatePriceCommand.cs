namespace Jobee.Pricing.Contracts.Commands;

public record CalculatePriceCommand
{
    public required Guid ProductId { get; init; }
    
    public required DateTimeOffset? PurchasedAt { get; init; }
}