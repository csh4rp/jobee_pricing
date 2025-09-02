using AwesomeAssertions;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.Events;
using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.UnitTests.Domain;

public class ProductTests
{
    private static readonly Guid DefaultProductId = Guid.Parse("0D62AF6F-C7A2-49D8-A89E-B923C6193C58");
    private static readonly Guid DefaultPriceId = Guid.Parse("E4A77469-E31B-41E4-B0AE-2F6BF581AE25");
    private static readonly Guid FuturePriceId = Guid.Parse("A1B2C3D4-E31B-41E4-B0AE-2F6BF581AE25");
    
    [Fact]
    public void ShouldGenerateEvent_WhenCreatingProduct()
    {
        var product = new Product(Guid.NewGuid(), "Test Product", 100, true, [
            new Price(Guid.NewGuid(), new DateTimeRange(), 100)
        ]);

        var events = product.DequeueEvents().ToList();

        events.Should().ContainSingle();
        events[0].Should().BeOfType<ProductCreated>();
        var @event = (ProductCreated)events[0];
        @event.Name.Should().Be(product.Name);
        @event.IsActive.Should().Be(product.IsActive);
        @event.NumberOfOffers.Should().Be(product.NumberOfOffers);
        @event.Prices.Should().HaveCount(1);
        @event.Prices[0].Amount.Should().Be(100);
        @event.Prices[0].DateTimeRange.StartsAt.Should().BeNull();
        @event.Prices[0].DateTimeRange.EndsAt.Should().BeNull();
    }
    
    [Fact]
    public void ShouldGenerateEventsAndUpdateProduct_WhenUpdatingProductProperties()
    {
        // Arrange
        var product = AnExistingProduct();
        const string newName = "New Name";
        const int newNumberOfOffers = 99999;
        var newPriceId = Guid.NewGuid();
        var prices = new List<Price>
        {
            new(DefaultPriceId, new DateTimeRange(), 99),
            new(newPriceId, new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(-1), null), 100)
        };
        
        // Act
        product.Update(newName, newNumberOfOffers, true, prices);
        
        var events = product.DequeueEvents().ToList();
        
        // Assert
        product.Name.Should().Be(newName);
        product.NumberOfOffers.Should().Be(newNumberOfOffers);
        product.IsActive.Should().BeTrue();
        events.Should().HaveCount(5);

        var productChangedEvent = (ProductChanged?)events.FirstOrDefault(e => e is ProductChanged);
        productChangedEvent.Should().NotBeNull();
        productChangedEvent.Name.Should().Be(newName);
        productChangedEvent.NumberOfOffers.Should().Be(newNumberOfOffers);
        
        var productActivatedEvent = (ProductActivated?)events.FirstOrDefault(e => e is ProductActivated);
        productActivatedEvent.Should().NotBeNull();
        
        var priceCreatedEvent = (PriceCreated?)events.FirstOrDefault(e => e is PriceCreated);
        priceCreatedEvent.Should().NotBeNull();
        priceCreatedEvent.Id.Should().Be(newPriceId);
        priceCreatedEvent.Amount.Should().Be(100);
        priceCreatedEvent.DateTimeRange.Should().Be(new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(-1), null));

        var priceChangedEvent = (PriceChanged?)events.FirstOrDefault(e => e is PriceChanged);
        priceChangedEvent.Should().NotBeNull();
        priceChangedEvent.Id.Should().Be(DefaultPriceId);
        priceChangedEvent.Amount.Should().Be(99);
        priceChangedEvent.DateTimeRange.Should().Be(new DateTimeRange());
        
        var priceRemovedEvent = (PriceRemoved?)events.FirstOrDefault(e => e is PriceRemoved);
        priceRemovedEvent.Should().NotBeNull();
        priceRemovedEvent.Id.Should().Be(FuturePriceId);
        priceRemovedEvent.Amount.Should().Be(90);
        priceRemovedEvent.DateTimeRange.Should().Be(new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(10), null));
    }
    
    [Fact]
    public void ShouldGenerateDeactivationEvent_WhenProductIsDeactivated()
    {
        // Arrange
        var product = AnExistingProduct(isActive: true);
        
        // Act
        product.Update(product.Name, product.NumberOfOffers, false, product.Prices);
        
        var events = product.DequeueEvents().ToList();

        events.Should().ContainSingle();
        events[0].Should().BeOfType<ProductDeactivated>();
        var @event = (ProductDeactivated)events[0];
    }
    
    [Fact]
    public void ShouldCreateProduct_WhenProductCreateEventIsApplied()
    {
        var @event = new ProductCreated(
            DefaultProductId,
            "Test Product",
            100,
            true,
            [
                new Price(Guid.NewGuid(), new DateTimeRange(), 100)
            ]);
        
        var product = new Product(@event);
        
        product.Id.Should().Be(@event.Id);
        product.Name.Should().Be(@event.Name);
        product.NumberOfOffers.Should().Be(@event.NumberOfOffers);
        product.IsActive.Should().Be(@event.IsActive);
        product.Prices.Should().HaveCount(1);
        product.Prices[0].Id.Should().Be(@event.Prices[0].Id);
        product.Prices[0].Amount.Should().Be(@event.Prices[0].Amount);
        product.Prices[0].DateTimeRange.Should().Be(@event.Prices[0].DateTimeRange);
    }
    
    [Fact]
    public void ShouldUpdateProduct_WhenProductChangedEventIsApplied()
    {
        // Arrange
        var product = AnExistingProduct();

        var @event = new ProductChanged("New Name", 99);
        
        // Act
        product.Apply(@event);

        // Assert
        product.Name.Should().Be(@event.Name);
        product.NumberOfOffers.Should().Be(@event.NumberOfOffers);
    }
    
    [Fact]
    public void ShouldActivateProduct_WhenProductActivatedEventIsApplied()
    {
        // Arrange
        var product = AnExistingProduct(isActive: false);

        var @event = new ProductActivated();
        
        // Act
        product.Apply(@event);

        // Assert
        product.IsActive.Should().BeTrue(); 
    }

    [Fact]
    public void ShouldDeactivateProduct_WhenProductDeactivatedEventIsApplied()
    {
        // Arrange
        var product = AnExistingProduct(isActive: true);

        var @event = new ProductDeactivated();
        
        // Act
        product.Apply(@event);

        // Assert
        product.IsActive.Should().BeFalse(); 
    }

    [Fact]
    public void ShouldAddPrice_WhenPriceCreatedEventIsApplied()
    {
        // Arrange
        var product = AnExistingProduct();

        var @event = new PriceCreated(Guid.NewGuid(), new DateTimeRange(DateTimeOffset.UtcNow.AddDays(100), null), 100);
        
        // Act
        product.Apply(@event);

        // Assert
        product.Prices.Should().Contain(p => p.Id == @event.Id);
    }

    [Fact]
    public void ShouldUpdatePrice_WhenPriceChangedEventIsApplied()
    {
        // Arrange
        var product = AnExistingProduct();

        var @event = new PriceChanged(FuturePriceId, new DateTimeRange(DateTimeOffset.UtcNow.AddDays(100), null), 100);
        
        // Act
        product.Apply(@event);

        // Assert
        product.Prices.Should().Contain(p => p.Id == @event.Id
            && p.Amount == @event.Amount
            && p.DateTimeRange == @event.DateTimeRange);
    }
    
    [Fact]
    public void ShouldDeletePrice_WhenPriceRemovedEventIsApplied()
    {
        // Arrange
        var product = AnExistingProduct();

        var @event = new PriceRemoved(FuturePriceId, new DateTimeRange(DateTimeOffset.UtcNow.AddDays(100), null), 100);
        
        // Act
        product.Apply(@event);

        // Assert
        product.Prices.Should().NotContain(p => p.Id == @event.Id);
    }
    
    
    
    private static Product AnExistingProduct(bool isActive = false)
    {
        var product = new Product(DefaultProductId,"Test Product", 100, isActive, [
            new Price(DefaultPriceId, new DateTimeRange(), 100),
            new Price(FuturePriceId, new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(10), null), 90),
        ]);

        _ = product.DequeueEvents().ToList();
        
        return product;
    }
    

}