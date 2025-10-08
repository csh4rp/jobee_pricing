using AwesomeAssertions;
using Jobee.Pricing.Application.Products.Calculation;
using Jobee.Pricing.Contracts.Products.Calculation;
using Jobee.Pricing.Contracts.Products.Common;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Jobee.Pricing.UnitTests.Fixtures;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Products.PriceCalculation;

[Collection("Products")]
public class CalculateProductPriceCommandHandlerTests
{
    private static readonly DateTimeOffset CurrentDate = new(2025, 9, 1, 0, 0, 0, TimeSpan.Zero);
    
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ISettingRepository _settingRepository = Substitute.For<ISettingRepository>();
    private readonly ProductFixture _productFixture;
    private readonly TestTimeProvider _testTimeProvider;
    private readonly CurrencyConverter _currencyConverter;

    public CalculateProductPriceCommandHandlerTests(ProductFixture productFixture, TestTimeProvider testTimeProvider)
    {
        _productFixture = productFixture;
        _testTimeProvider = testTimeProvider;
        _testTimeProvider.CurrentDate = CurrentDate;
        _currencyConverter = new CurrencyConverter(_settingRepository);
        _settingRepository.FindSettingAsync("EXCHANGE_RATE_PLN_USD", Arg.Any<CancellationToken>())
            .Returns(new Setting("EXCHANGE_RATE_EUR_USD", "0.25"));
    }

    [Fact]
    public async Task ShouldCalculateCorrectPrice_WhenDefaultCurrencyIsUsed()
    {
        // Arrange
        var product = _productFixture.AProduct();
        
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);
        
        var command = new CalculateProductPriceCommand
        {
            ProductId = product.Id,
            Currency = CurrencyModel.PLN
        };
        
        // Act
        var result = await CalculateProductPriceCommandHandler.Handle(command,
            _productRepository, _currencyConverter, _testTimeProvider, CancellationToken.None);
        
        // Assert
        result.Amount.Should().Be(_productFixture.CurrentPrice.Amount);
        result.Currency.Should().Be(CurrencyModel.PLN);
    }
    
    [Fact]
    public async Task ShouldCalculateCorrectPrice_WhenCurrencyConversionIsRequired()
    {
        // Arrange
        var product = _productFixture.AProduct();
        
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);
        
        var command = new CalculateProductPriceCommand
        {
            ProductId = product.Id,
            Currency = CurrencyModel.USD,
        };
        
        // Act
        var result = await CalculateProductPriceCommandHandler.Handle(command, _productRepository, _currencyConverter,
            _testTimeProvider, CancellationToken.None);
        
        // Assert
        result.Amount.Should().Be(_productFixture.CurrentPrice.Amount / 4);
        result.Currency.Should().Be(CurrencyModel.USD);
    }
}