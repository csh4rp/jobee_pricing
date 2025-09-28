namespace Jobee.Pricing.Domain.Common;

public class PriceValidator
{
    public static void ValidatePrices(IReadOnlyCollection<Price> prices)
    {
        if (prices.Count(p => p.IsDefault) != 1)
        {
            throw new ArgumentException("Only one default price is allowed.");
        }
        
        foreach (var price in prices)
        {
            if (prices.Any(p => p.Id != price.Id && !p.IsDefault && !price.IsDefault && p.DateTimeRange.Overlaps(price.DateTimeRange)))
            {
                throw new ArgumentException($"Price with ID {price.Id} overlaps with another price in the collection.");
            }
        }
    }
}