using Jobee.Pricing.Application.Products.Modification;
using Jobee.Pricing.Contracts.Products.Common;
using Jobee.Pricing.Contracts.Products.Modification;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Jobee.Pricing.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Products.Modification;

[Collection("Products")]
public class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ISettingRepository _settingRepository = Substitute.For<ISettingRepository>();
    private readonly ILogger<UpdateProductCommandHandler> _logger = Substitute.For<ILogger<UpdateProductCommandHandler>>();
    private readonly SettingsService _settingsService;
    private readonly ProductFixture _fixture;

    public UpdateProductCommandHandlerTests(ProductFixture fixture)
    {
        _fixture = fixture;
        _settingsService = new SettingsService(_settingRepository);
    }
    
    [Fact]
    public async Task ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var existingProduct = _fixture.AProduct();
        _productRepository.GetByIdAsync(existingProduct.Id, Arg.Any<CancellationToken>()).Returns(existingProduct);
        
        var command = new UpdateProductCommand
        {
            Name = "Updated Product",
            Description = "Updated Description",
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
            IsActive = false,
            Prices = [new UpdatePriceModel
            {
                Amount = 100,
                Id = Guid.NewGuid(),
                EndsAt = null,
                StartsAt = null
            }]
        };
        command.SetProductId(existingProduct.Id);
        
        // Act
        await UpdateProductCommandHandler.Handle(command, _productRepository, _settingsService, _logger, CancellationToken.None);
        
        // Assert
        await _productRepository.Received(1).UpdateAsync(Arg.Is<Product>(p => 
            p.Id == existingProduct.Id
            && p.Name == command.Name
            && p.Description == command.Description
            && p.IsActive == command.IsActive
            && p.FeatureFlags.HasPriority == command.FeatureFlags.HasPriority
            && p.Attributes.NumberOfBumps == command.Attributes.NumberOfBumps
            && p.Attributes.NumberOfLocations == command.Attributes.NumberOfLocations
            && p.Attributes.Duration == TimeSpan.FromDays(command.Attributes.DurationInDays)
            ), Arg.Any<CancellationToken>());
    }
}