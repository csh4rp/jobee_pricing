using AwesomeAssertions;
using Jobee.Pricing.Application.Packages.Creation;
using Jobee.Pricing.Contracts.Packages.Creation;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Jobee.Utils.Application.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Packages.Creation;

public class CreatePackageCommandHandlerTests
{
    private static readonly Guid ExistingProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IPackageRepository _packageRepository = Substitute.For<IPackageRepository>();
    private readonly ISettingRepository _settingRepository = Substitute.For<ISettingRepository>();
    private readonly ILogger<CreatePackageCommandHandler> _logger = Substitute.For<ILogger<CreatePackageCommandHandler>>();
    private readonly SettingsService _settingsService;

    public CreatePackageCommandHandlerTests() => _settingsService = new SettingsService(_settingRepository);

    [Fact]
    public async Task ShouldCreatePackage_WhenProductExists()
    {
        _productRepository.ExistsAsync(ExistingProductId, Arg.Any<CancellationToken>()).Returns(true);
        
        var command = new CreatePackageCommand
        {
            Name = "Test Package",
            Description = "Description",
            ProductId = ExistingProductId,
            IsActive = true,
            Quantity = 1,
            Prices = new List<Contracts.Common.CreatePriceModel>
            {
                new()
                {
                    Amount = 10,
                    EndsAt = null,
                    StartsAt = null
                }
            }
        };
        
        var result = await CreatePackageCommandHandler.Handle(command, 
            _packageRepository, _productRepository, _settingsService, _logger, CancellationToken.None);
        
        result.Should().NotBeNull();

        await _packageRepository.Received(1)
            .AddAsync(Arg.Is<Package>(p => p.Id == result.Id
            && p.IsActive == command.IsActive
            && p.Name == command.Name
            && p.Description == command.Description
            && p.Quantity == command.Quantity
            && p.Prices[0].IsDefault
            && p.Prices[0].DateTimeRange.StartsAt == command.Prices[0].StartsAt
            && p.Prices[0].DateTimeRange.EndsAt == command.Prices[0].EndsAt
            ), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task ShouldThrowException_WhenProductDoesNotExist()
    {
        _productRepository.ExistsAsync(ExistingProductId, Arg.Any<CancellationToken>()).Returns(false);
        
        var command = new CreatePackageCommand
        {
            Name = "Test Package",
            Description = "Description",
            ProductId = ExistingProductId,
            IsActive = true,
            Quantity = 1,
            Prices = new List<Contracts.Common.CreatePriceModel>
            {
                new()
                {
                    Amount = 10,
                    EndsAt = null,
                    StartsAt = null
                }
            }
        };

        var exception = await Assert.ThrowsAsync<ValidationException>(async () => await
            CreatePackageCommandHandler.Handle(command, _packageRepository, _productRepository, _settingsService,
                _logger, CancellationToken.None));

        exception.Errors.Should().HaveCount(1);
        exception.Errors[0].Target.Should().Be("ProductId");
    }
}