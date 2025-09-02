using Jobee.Pricing.Application.Modification;
using Jobee.Pricing.Contracts.Commands;
using Jobee.Pricing.Contracts.Models;
using Jobee.Pricing.Contracts.Modification;
using Jobee.Pricing.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Handlers;

public class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<UpdateProductCommandHandler> _logger = Substitute.For<ILogger<UpdateProductCommandHandler>>();

    [Fact]
    public async Task ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var existingProduct = AProduct();
        _repository.GetByIdAsync(existingProduct.Id, Arg.Any<CancellationToken>()).Returns(existingProduct);
        
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
        await UpdateProductCommandHandler.Handle(command, _repository, _logger, CancellationToken.None);
        
        // Assert
        await _repository.Received(1).UpdateAsync(Arg.Is<Product>(p => 
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
        []);
}