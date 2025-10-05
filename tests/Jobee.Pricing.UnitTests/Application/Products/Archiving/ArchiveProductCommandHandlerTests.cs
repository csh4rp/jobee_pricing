using Jobee.Pricing.Application.Products.Archiving;
using Jobee.Pricing.Contracts.Products.Archiving;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Products.Archiving;

[Collection("Products")]
public class ArchiveProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IPackageRepository _packageRepository = Substitute.For<IPackageRepository>();
    private readonly ILogger<ArchiveProductCommandHandler> _logger = Substitute.For<ILogger<ArchiveProductCommandHandler>>();
    private readonly ProductFixture _fixture;

    public ArchiveProductCommandHandlerTests(ProductFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task ShouldArchiveProduct_WhenProductExists()
    {
        // Arrange
        var product = _fixture.AProduct();
        
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _packageRepository.ExistsForProductAsync(product.Id, Arg.Any<CancellationToken>()).Returns(false);
        
        var command = new ArchiveProductCommand(product.Id);
        
        // Act
        await ArchiveProductCommandHandler.Handle(command, _productRepository, _packageRepository, _logger, CancellationToken.None);
        
        // Assert
        await _productRepository.Received(1).ArchiveAsync(Arg.Is<Product>(p => 
            p.Id == product.Id), Arg.Any<CancellationToken>());
    }
}