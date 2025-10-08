using Jobee.Pricing.Contracts.Packages.Modification;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Settings;
using Microsoft.Extensions.Logging;

namespace Jobee.Pricing.Application.Packages.Modification;

public class UpdatePackageCommandHandler
{
    public static async Task Handle(UpdatePackageCommand command,
        IPackageRepository packageRepository,
        SettingsService settingsService,
        ILogger<UpdatePackageCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var defaultCurrency = await settingsService.GetDefaultCurrencyAsync(cancellationToken);
        var package = await packageRepository.GetByIdAsync(command.PackageId, cancellationToken);
        
        var prices = command.Prices.Select(p => 
            new Price(p.Id ?? Guid.CreateVersion7(), new DateTimeRange(p.StartsAt, p.EndsAt), 
                new Money(p.Amount, defaultCurrency))
        ).ToList();
        
        package.Update(command.Name, command.Description, command.IsActive, command.Quantity, prices);
        
        await packageRepository.UpdateAsync(package, cancellationToken);
        
        logger.LogInformation("Package with id: {id} updated", package.Id);
    }
}