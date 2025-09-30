using Jobee.Pricing.Contracts.Products.Models;
using Jobee.Pricing.Contracts.Products.PriceCalculation;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;

namespace Jobee.Pricing.Application.Products.Calculation;

public class CalculateProductPriceCommandHandler
{
    public static async Task<PriceCalculationResult> Handle(CalculateProductPriceCommand request,
        IProductRepository productRepository,
        CurrencyConverter currencyConverter,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        var price = product.GetPrice(timeProvider.GetUtcNow());
        var currency = Enum.Parse<Currency>(request.Currency.ToString(), true);

        var calculatedPrice = await currencyConverter.ConvertAsync(price.Money, currency, cancellationToken);

        return new PriceCalculationResult(calculatedPrice.Amount, request.Currency);
    }
}