using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.Domain.Settings;

public class SettingsService
{
    private readonly ISettingRepository _settingRepository;

    public SettingsService(ISettingRepository settingRepository)
    {
        _settingRepository = settingRepository;
    }

    public async Task<Currency> GetDefaultCurrencyAsync(CancellationToken cancellationToken)
    {
        var defaultCurrencySetting = await _settingRepository.FindSettingAsync(SettingNames.DefaultCurrency, cancellationToken);
        return defaultCurrencySetting is null ? Currency.EUR : Enum.Parse<Currency>(defaultCurrencySetting.Value, true);
    }
}