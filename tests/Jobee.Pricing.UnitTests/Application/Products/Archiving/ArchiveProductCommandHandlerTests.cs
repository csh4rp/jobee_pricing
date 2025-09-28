using Jobee.Pricing.Application.Products.Archiving;
using Jobee.Pricing.Contracts.Products.Archiving;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Products.Archiving;

[Collection("Products")]
public class ArchiveProductCommandHandlerTests
{
    
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<ArchiveProductCommandHandler> _logger = Substitute.For<ILogger<ArchiveProductCommandHandler>>();
    private readonly ProductFixture _fixture;

    public ArchiveProductCommandHandlerTests(ProductFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldArchiveProduct_WhenProductExists()
    {
        // Arrange
        var product = _fixture.AProduct();
        
        _repository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        
        var command = new ArchiveProductCommand(product.Id);
        
        // Act
        await ArchiveProductCommandHandler.Handle(command, _repository, _logger, CancellationToken.None);
        
        // Assert
        await _repository.Received(1).ArchiveAsync(Arg.Is<Product>(p => 
            p.Id == product.Id), Arg.Any<CancellationToken>());
    }
}