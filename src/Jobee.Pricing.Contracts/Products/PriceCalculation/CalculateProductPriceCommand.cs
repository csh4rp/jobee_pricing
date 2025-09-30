using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Products.PriceCalculation;

public record CalculateProductPriceCommand
{
    public required Guid ProductId { get; init; }
    public required CurrencyModel Currency { get; init; }
}