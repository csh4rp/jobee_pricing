namespace Jobee.Pricing.Domain.Products;

public class PriceValidationException : Exception
{
    public PriceValidationException(Guid priceId, string message) : base(message)
    {
    }
}