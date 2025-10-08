using Jobee.Pricing.Application.Packages.Archiving;
using Jobee.Pricing.Contracts.Packages.Archiving;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Packages.Archiving;

[Collection("Packages")]
public class ArchivePackageCommandHandlerTests
{
    private readonly IPackageRepository _packageRepository = Substitute.For<IPackageRepository>();
    private readonly ILogger<ArchivePackageCommandHandler> _logger = Substitute.For<ILogger<ArchivePackageCommandHandler>>();
    private readonly PackageFixture _packageFixture;

    public ArchivePackageCommandHandlerTests(PackageFixture packageFixture) => _packageFixture = packageFixture;

    [Fact]
    public async Task ShouldArchivePackage_WhenPackageExists()
    {
        var package = _packageFixture.APackage();
        
        _packageRepository.GetByIdAsync(package.Id, Arg.Any<CancellationToken>())
            .Returns(package);

        var command = new ArchivePackageCommand(package.Id);
        
        await ArchivePackageCommandHandler.Handle(command, 
            _packageRepository, _logger, CancellationToken.None);
        
        await _packageRepository.Received(1)
            .ArchiveAsync(package, Arg.Any<CancellationToken>());
    }
    
}