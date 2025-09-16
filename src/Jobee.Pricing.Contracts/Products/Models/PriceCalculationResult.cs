using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Contracts.Products.Models;

public record PriceCalculationResult(decimal Amount, CurrencyModel Currency);