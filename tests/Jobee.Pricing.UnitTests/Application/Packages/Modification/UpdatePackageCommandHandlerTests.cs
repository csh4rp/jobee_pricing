using Jobee.Pricing.Application.Packages.Creation;
using Jobee.Pricing.Application.Packages.Modification;
using Jobee.Pricing.Application.Products.Modification;
using Jobee.Pricing.Contracts.Packages.Modification;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Settings;
using Jobee.Pricing.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Packages.Modification;

[Collection("Packages")]
public class UpdatePackageCommandHandlerTests
{
    private readonly IPackageRepository _packageRepository = Substitute.For<IPackageRepository>();
    private readonly ISettingRepository _settingRepository = Substitute.For<ISettingRepository>();
    private readonly ILogger<UpdatePackageCommandHandler> _logger = Substitute.For<ILogger<UpdatePackageCommandHandler>>();
    private readonly SettingsService _settingsService;
    private readonly PackageFixture _packageFixture;
    
    public UpdatePackageCommandHandlerTests(PackageFixture packageFixture)
    {
        _packageFixture = packageFixture;
        _settingsService = new SettingsService(_settingRepository);
    }

    [Fact]
    public async Task ShouldUpdatePackage_WhenAll()
    {
        var existingPackage = _packageFixture.APackage();
        _packageRepository.GetByIdAsync(existingPackage.Id, Arg.Any<CancellationToken>())
            .Returns(existingPackage);
        
        var command = new UpdatePackageCommand
        {
            PackageId = existingPackage.Id,
            Name = "Test Package",
            Description = "Description",
            ProductId = existingPackage.ProductId,
            IsActive = true,
            Quantity = 2,
            Prices = new List<Contracts.Common.UpdatePriceModel>
            {
                new()
                {
                    Id = null,
                    Amount = 10,
                    EndsAt = null,
                    StartsAt = null
                },
                new()
                {
                    Id = existingPackage.Prices[0].Id,
                    Amount = 20,
                    StartsAt = DateTimeOffset.UtcNow.AddDays(1),
                    EndsAt = null
                }
            }
        };

        await UpdatePackageCommandHandler.Handle(command,
            _packageRepository,
            _settingsService,
            _logger,
            CancellationToken.None);

        await _packageRepository.Received(1).UpdateAsync(Arg.Is<Package>(p =>
            p.Id == command.PackageId 
            && p.Name == command.Name
            && p.Description == command.Description
            && p.IsActive == command.IsActive
            && p.Quantity == command.Quantity
            && p.Prices.Count == command.Prices.Count
            && p.Prices.Any(pr => pr.Id == command.Prices[1].Id && pr.Value.Amount == command.Prices[1].Amount)
            ), Arg.Any<CancellationToken>());
    }
}