using Jobee.Pricing.Contracts.Packages.Archiving;
using Jobee.Pricing.Domain.Packages;
using Microsoft.Extensions.Logging;

namespace Jobee.Pricing.Application.Packages.Archiving;

public class ArchivePackageCommandHandler
{
    public static async Task Handle(ArchivePackageCommand command,
        IPackageRepository packageRepository,
        ILogger<ArchivePackageCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var package = await packageRepository.GetByIdAsync(command.PackageId, cancellationToken);

        await packageRepository.ArchiveAsync(package, cancellationToken);
        
        logger.LogInformation("Package with id: {id} archived", package.Id);
    }
}