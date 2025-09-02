namespace Jobee.Pricing.Domain.Exceptions;

public class PriceValidationException : Exception
{
    public PriceValidationException(Guid priceId, string message) : base(message)
    {
        
    }
}