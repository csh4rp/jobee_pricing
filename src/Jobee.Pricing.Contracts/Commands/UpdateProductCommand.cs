using Jobee.Pricing.Contracts.Models;

namespace Jobee.Pricing.Contracts.Commands;

public record UpdateProductCommand
{
    public Guid ProductId { get; private set; }

    public required string Name { get; init; }
    
    public required int NumberOfOffers { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required IReadOnlyList<PriceModel> Prices { get; init; }
    
    public void SetProductId(Guid id) => ProductId = id;
}