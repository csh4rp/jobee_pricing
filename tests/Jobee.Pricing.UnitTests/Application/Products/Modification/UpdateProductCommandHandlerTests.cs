using Jobee.Pricing.Application.Products.Modification;
using Jobee.Pricing.Contracts.Products.Modification;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Products.Modification;

public class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ISettingRepository _settingRepository = Substitute.For<ISettingRepository>();
    private readonly ILogger<UpdateProductCommandHandler> _logger = Substitute.For<ILogger<UpdateProductCommandHandler>>();
    private readonly SettingsService _settingsService;

    public UpdateProductCommandHandlerTests()
    {
        _settingsService = new SettingsService(_settingRepository);
    }
    
    [Fact]
    public async Task ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var existingProduct = AProduct();
        _productRepository.GetByIdAsync(existingProduct.Id, Arg.Any<CancellationToken>()).Returns(existingProduct);
        
        var command = new UpdateProductCommand
        {
            Name = "Updated Product",
            NumberOfOffers = 2,
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
            && p.NumberOfOffers == command.NumberOfOffers
            && p.IsActive == command.IsActive), Arg.Any<CancellationToken>());
    }
    
    private static Product AProduct() => new(
        Guid.NewGuid(),
        "Test Product",
        1,
        true,
        [new Price(Guid.CreateVersion7(), new DateTimeRange(), new Money(100, Currency.EUR))
        ]);
}