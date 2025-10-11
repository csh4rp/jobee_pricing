using Jobee.Pricing.Contracts.Packages.Calculation;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Settings;

namespace Jobee.Pricing.Application.Packages.Calculation;

public class CalculatePackagePriceCommandHandler
{
    public static async Task<PackagePriceCalculationResult> Handle(CalculatePackagePriceCommand command,
        IPackageRepository packageRepository,
        CurrencyConverter currencyConverter,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var package = await packageRepository.GetByIdAsync(command.PackageId, cancellationToken);

        var price = package.GetPrice(timeProvider.GetUtcNow());
        
        var currency = Enum.Parse<Currency>(command.Currency.ToString(), true);
        var convertedPrice = await currencyConverter.ConvertAsync(price.Value, currency, cancellationToken);

        return new PackagePriceCalculationResult
        {
            TotalAmount = convertedPrice.Amount,
            Currency = command.Currency,
            ProductId = package.ProductId,
            Quantity = package.Quantity,
            UnitAmount = Math.Round(convertedPrice.Amount / package.Quantity, 2)
        };
    }
}