using AwesomeAssertions;
using Jobee.Pricing.Application.Packages.Calculation;
using Jobee.Pricing.Contracts.Packages.Calculation;
using Jobee.Pricing.Contracts.Products.Common;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Settings;
using Jobee.Pricing.UnitTests.Fixtures;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Packages.PriceCalculation;

[Collection("Packages")]
public class CalculatePackagePriceCommandHandlerTests
{
    private readonly IPackageRepository _packageRepository = Substitute.For<IPackageRepository>();
    private readonly ISettingRepository _settingRepository = Substitute.For<ISettingRepository>();
    private readonly CurrencyConverter _currencyConverter;
    private readonly PackageFixture _packageFixture;
    private readonly TestTimeProvider _timeProvider;

    public CalculatePackagePriceCommandHandlerTests(PackageFixture packageFixture)
    {
        _packageFixture = packageFixture;
        _timeProvider = new TestTimeProvider
        {
            CurrentDate = new DateTime(2025, 10, 1)
        };
        _currencyConverter = new CurrencyConverter(_settingRepository);
        _settingRepository.FindSettingAsync("EXCHANGE_RATE_PLN_USD", Arg.Any<CancellationToken>())
            .Returns(new Setting("EXCHANGE_RATE_PLN_USD", "0.25"));
    }

    [Fact]
    public async Task ShouldCalculatePrice_WhenPackageExists()
    {
        var package = _packageFixture.APackage();
        _packageRepository.GetByIdAsync(package.Id, Arg.Any<CancellationToken>()).Returns(package);
        
        var command = new CalculatePackagePriceCommand
        {
            PackageId = package.Id,
            Currency = CurrencyModel.USD
        };
        
        var result = await CalculatePackagePriceCommandHandler.Handle(command, 
            _packageRepository, _currencyConverter, _timeProvider, CancellationToken.None);
        
        result.TotalAmount.Should().Be(_packageFixture.CurrentPrice.Amount * 0.25m);
        result.Currency.Should().Be(CurrencyModel.USD);
        result.ProductId.Should().Be(package.ProductId);
        result.Quantity.Should().Be(package.Quantity);
    }
   
}