using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Products.Models;

public record ProductPriceCalculationResult(decimal Amount, CurrencyModel Currency);