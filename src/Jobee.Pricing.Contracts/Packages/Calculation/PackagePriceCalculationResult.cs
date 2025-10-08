using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Packages.Calculation;

public record PackagePriceCalculationResult
{
    public required decimal TotalAmount { get; init; }
    
    public required decimal UnitAmount { get; init; }
    
    public required CurrencyModel Currency { get; init; }
    
    public required Guid ProductId { get; init; }
    
    public required int Quantity { get; init; }
}