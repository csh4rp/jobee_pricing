using Jobee.Pricing.Application.Archiving;
using Jobee.Pricing.Contracts.Archiving;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.Entities;
using Jobee.Pricing.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Archiving;

public class ArchiveProductCommandHandlerTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<ArchiveProductCommandHandler> _logger = Substitute.For<ILogger<ArchiveProductCommandHandler>>();

    [Fact]
    public async Task ShouldArchiveProduct_WhenProductExists()
    {
        // Arrange
        var product = AProduct();
        
        _repository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        
        var command = new ArchiveProductCommand(product.Id);
        
        // Act
        await ArchiveProductCommandHandler.Handle(command, _repository, _logger, CancellationToken.None);
        
        // Assert
        await _repository.Received(1).ArchiveAsync(Arg.Is<Product>(p => 
            p.Id == product.Id), Arg.Any<CancellationToken>());
    }
    
    private static Product AProduct() => new(
        Guid.NewGuid(),
        "Test Product",
        1,
        true,
        [new Price(Guid.CreateVersion7(), new DateTimeRange(), 100)]);

}