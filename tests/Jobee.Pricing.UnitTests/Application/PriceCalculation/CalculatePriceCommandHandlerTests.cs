using AwesomeAssertions;
using Jobee.Pricing.Application.Calculation;
using Jobee.Pricing.Contracts.PriceCalculation;
using Jobee.Pricing.Domain;
using Jobee.Pricing.UnitTests.Fixtures;
using Jobee.Utils.Application.Exceptions;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.PriceCalculation;

[Collection("Products")]
public class CalculatePriceCommandHandlerTests
{
    private static readonly DateTimeOffset CurrentDate = new(2025, 9, 1, 0, 0, 0, TimeSpan.Zero);
    
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ProductFixture _productFixture;
    private readonly TestTimeProvider _testTimeProvider;

    public CalculatePriceCommandHandlerTests(ProductFixture productFixture, TestTimeProvider testTimeProvider)
    {
        _productFixture = productFixture;
        _testTimeProvider = testTimeProvider;
        _testTimeProvider.CurrentDate = CurrentDate;
    }

    [Fact]
    public async Task ShouldCalculateCorrectPrice_WhenIdWithoutVersionIsUsed()
    {
        // Arrange
        var product = _productFixture.AProduct();
        
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);
        
        var command = new CalculatePriceCommand(product.Id.ToString());
        
        // Act
        var result = await CalculatePriceCommandHandler.Handle(command, _productRepository, _testTimeProvider, CancellationToken.None);
        
        // Assert
        result.Amount.Should().Be(_productFixture.CurrentPrice);
    }
    
    [Fact]
    public async Task ShouldCalculateCorrectPrice_WhenIdWithVersionIsUsed()
    {
        // Arrange
        var product = _productFixture.AProduct();
        var version = product.Version;
        
        _productRepository.GetByIdAsync(product.Id, version.Number, Arg.Any<CancellationToken>())
            .Returns(product);
        
        var command = new CalculatePriceCommand(version.ToString());
        
        // Act
        var result = await CalculatePriceCommandHandler.Handle(command, _productRepository, _testTimeProvider, CancellationToken.None);
        
        // Assert
        result.Amount.Should().Be(_productFixture.CurrentPrice);
    }

    [Fact]
    public async Task ShouldThrowValidationException_WhenProductIdIsInvalid()
    {
        // Arrange
        var command = new CalculatePriceCommand("invalid-id");
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await CalculatePriceCommandHandler.Handle(command, _productRepository, _testTimeProvider, CancellationToken.None));
    }
}