using AwesomeAssertions;
using Jobee.Pricing.Application.Products.Creation;
using Jobee.Pricing.Contracts.Products.Common;
using Jobee.Pricing.Contracts.Products.Creation;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Products.Creation;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ISettingRepository _settingRepository = Substitute.For<ISettingRepository>();
    private readonly ILogger<CreateProductCommandHandler> _logger = Substitute.For<ILogger<CreateProductCommandHandler>>();
    private readonly SettingsService _settingsService;

    public CreateProductCommandHandlerTests()
    {
        _settingsService = new SettingsService(_settingRepository);
    }
    
    [Fact]
    public async Task ShouldCreateProduct_WhenAllPropertiesArePresent()
    {
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Description",
            IsActive = true,
            Attributes = new AttributesModel
            {
                NumberOfBumps = 1,
                NumberOfLocations = 1,
                DurationInDays = 30
            },
            FeatureFlags = new FeatureFlagsModel
            {
                HasPriority = false
            },
            Prices = new List<CreatePriceModel>
            {
                new()
                {
                    Amount = 10,
                    EndsAt = null,
                    StartsAt = null
                }
            }
        };

        var response = await CreateProductCommandHandler.Handle(command, _productRepository, _settingsService, _logger, CancellationToken.None);
        
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
        await _productRepository.Received(1).AddAsync(Arg.Is<Product>(p => 
            p.IsActive == command.IsActive
            && p.Name == command.Name
            && p.Description == command.Description
            && p.Attributes.Duration == TimeSpan.FromDays(command.Attributes.DurationInDays)
            && p.Attributes.NumberOfBumps == command.Attributes.NumberOfBumps
            && p.Attributes.NumberOfLocations == command.Attributes.NumberOfLocations
            && p.FeatureFlags.HasPriority == command.FeatureFlags.HasPriority), Arg.Any<CancellationToken>());
    }
}