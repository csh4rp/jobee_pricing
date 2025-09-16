using AwesomeAssertions;
using Jobee.Pricing.Application.Products.Creation;
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
            NumberOfOffers = 1,
            IsActive = true,
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
            && p.NumberOfOffers == command.NumberOfOffers), Arg.Any<CancellationToken>());
    }
}