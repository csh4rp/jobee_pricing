namespace Jobee.Pricing.Domain.Settings;

public interface ISettingRepository
{
    Task<Setting?> FindSettingAsync(string name, CancellationToken cancellationToken);
}