using AwesomeAssertions;
using Jobee.Pricing.Application.Creation;
using Jobee.Pricing.Contracts.Creation;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jobee.Pricing.UnitTests.Application.Creation;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<CreateProductCommandHandler> _logger = Substitute.For<ILogger<CreateProductCommandHandler>>();
    
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

        var response = await CreateProductCommandHandler.Handle(command, _repository, _logger, CancellationToken.None);
        
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Is<Product>(p => 
            p.IsActive == command.IsActive
            && p.Name == command.Name
            && p.NumberOfOffers == command.NumberOfOffers), Arg.Any<CancellationToken>());
    }
}