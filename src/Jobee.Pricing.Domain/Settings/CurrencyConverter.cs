using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Utils.Application.Exceptions;

namespace Jobee.Pricing.Domain.Settings;

public class CurrencyConverter
{
    private ISettingRepository _settingRepository;

    public CurrencyConverter(ISettingRepository settingRepository) => _settingRepository = settingRepository;

    public async ValueTask<Money> ConvertAsync(Money from, Currency toCurrency, CancellationToken cancellationToken)
    {
        if (from.Currency == toCurrency)
        {
            return from;
        }
        
        var rateSetting = await _settingRepository.FindSettingAsync($"EXCHANGE_RATE_{from.Currency}_{toCurrency}", cancellationToken)
            ?? throw new EntityNotFoundException("Exchange rate setting not found", $"EXCHANGE_RATE_{from.Currency}_{toCurrency}");

        var rate = decimal.Parse(rateSetting.Value);
        
        return new Money(from.Amount * rate, toCurrency);
    }
}