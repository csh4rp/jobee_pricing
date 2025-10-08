using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Packages.Calculation;

public record CalculatePackagePriceCommand
{
    public required Guid PackageId { get; init; }
    
    public required CurrencyModel Currency { get; init; }
}