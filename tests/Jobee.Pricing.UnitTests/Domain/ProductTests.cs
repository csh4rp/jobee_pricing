using AwesomeAssertions;
using Jobee.Pricing.Domain;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Events;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;

namespace Jobee.Pricing.UnitTests.Domain;

public class ProductTests
{
    private static readonly Guid DefaultProductId = Guid.Parse("0D62AF6F-C7A2-49D8-A89E-B923C6193C58");
    private static readonly Guid DefaultPriceId = Guid.Parse("E4A77469-E31B-41E4-B0AE-2F6BF581AE25");
    private static readonly Guid FuturePriceId = Guid.Parse("A1B2C3D4-E31B-41E4-B0AE-2F6BF581AE25");
    private const decimal DefaultPriceAmount = 100;
    private static readonly Money DefaultPrice = new(100, Currency.EUR);
    private static readonly Money FuturePrice = new(90, Currency.EUR);
    
    [Fact]
    public void ShouldGenerateEvent_WhenCreatingProduct()
    {
        var product = new Product( "Test Product", "Description", true,
        new FeatureFlags
        {
            HasPriority = false
        },
        new Attributes
        {
          NumberOfBumps  = 1,
          NumberOfLocations = 1,
          Duration = TimeSpan.FromDays(30)
        },
        [
            new Price(Guid.NewGuid(), new DateTimeRange(), DefaultPrice)
        ]);

        var events = product.DequeueEvents().ToList();

        events.Should().ContainSingle();
        events[0].Should().BeOfType<ProductCreated>();
        var @event = (ProductCreated)events[0];
        @event.Name.Should().Be(product.Name);
        @event.IsActive.Should().Be(product.IsActive);
        @event.Prices.Should().HaveCount(1);
        @event.Prices[0].Money.Amount.Should().Be(100);
        @event.Prices[0].DateTimeRange.StartsAt.Should().BeNull();
        @event.Prices[0].DateTimeRange.EndsAt.Should().BeNull();
    }
    
    [Fact]
    public void ShouldGenerateEventsAndUpdateProduct_WhenUpdatingProductProperties()
    {
        // Arrange
        var product = AnExistingProduct();
        const string newName = "New Name";
        const string newDescription = "New Desc";
        var newFeatureFlags = new FeatureFlags
        {
            HasPriority = false
        };
        var newAttributes = new Attributes
        {
            NumberOfBumps = 10,
            NumberOfLocations = 10,
            Duration = TimeSpan.FromDays(90)
        };
        var newPriceId = Guid.NewGuid();
        var prices = new List<Price>
        {
            new(DefaultPriceId, new DateTimeRange(), new Money(99, Currency.EUR)),
            new(newPriceId, new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(-1), null), DefaultPrice)
        };
        
        // Act
        product.Update(newName, newDescription, true, newFeatureFlags, newAttributes, prices);
        
        var events = product.DequeueEvents().ToList();
        
        // Assert
        product.Name.Should().Be(newName);
        product.IsActive.Should().BeTrue();
        events.Should().HaveCount(8);

        var productNameChangedEvent = (ProductNameChanged?)events.FirstOrDefault(e => e is ProductNameChanged);
        productNameChangedEvent.Should().NotBeNull();
        productNameChangedEvent.Name.Should().Be(newName);
        
        var productDescriptionChangedEvent = (ProductDescriptionChanged?)events.FirstOrDefault(e => e is ProductDescriptionChanged);
        productDescriptionChangedEvent.Should().NotBeNull();
        productDescriptionChangedEvent.Description.Should().Be(newDescription);
        
        var productActivatedEvent = (ProductActivated?)events.FirstOrDefault(e => e is ProductActivated);
        productActivatedEvent.Should().NotBeNull();
        
        var productAttributesChangedEvent = (ProductAttributesChanged?)events.FirstOrDefault(e => e is ProductAttributesChanged);
        productAttributesChangedEvent.Should().NotBeNull();
        
        var productFeatureFlagsChangedEvent = (ProductFeatureFlagsChanged?)events.FirstOrDefault(e => e is ProductFeatureFlagsChanged);
        productFeatureFlagsChangedEvent.Should().NotBeNull();
        
        var priceCreatedEvent = (ProductPriceCreated?)events.FirstOrDefault(e => e is ProductPriceCreated);
        priceCreatedEvent.Should().NotBeNull();
        priceCreatedEvent.Id.Should().Be(newPriceId);
        priceCreatedEvent.Money.Amount.Should().Be(100);
        priceCreatedEvent.DateTimeRange.Should().Be(new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(-1), null));

        var priceChangedEvent = (ProductPriceChanged?)events.FirstOrDefault(e => e is ProductPriceChanged);
        priceChangedEvent.Should().NotBeNull();
        priceChangedEvent.Id.Should().Be(DefaultPriceId);
        priceChangedEvent.Money.Amount.Should().Be(99);
        priceChangedEvent.DateTimeRange.Should().Be(new DateTimeRange());
        
        var priceRemovedEvent = (ProductPriceRemoved?)events.FirstOrDefault(e => e is ProductPriceRemoved);
        priceRemovedEvent.Should().NotBeNull();
        priceRemovedEvent.Id.Should().Be(FuturePriceId);
        priceRemovedEvent.Money.Amount.Should().Be(90);
        priceRemovedEvent.DateTimeRange.Should().Be(new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(10), null));
    }
    
    [Fact]
    public void ShouldGenerateDeactivationEvent_WhenProductIsDeactivated()
    {
        // Arrange
        var product = AnExistingProduct(isActive: true);
        
        // Act
        product.Update(product.Name, product.Description, false, product.FeatureFlags, product.Attributes, product.Prices);
        
        var events = product.DequeueEvents().ToList();

        events.Should().ContainSingle();
        events[0].Should().BeOfType<ProductDeactivated>();
        var @event = (ProductDeactivated)events[0];
        @event.Should().NotBeNull();
    }
    
    [Fact]
    public void ShouldCreateProduct_WhenProductCreateEventIsApplied()
    {
        var @event = new ProductCreated
        {
            ProductId = DefaultProductId,
            Name = "Test Product",
            Description = "Description",
            IsActive = true,
            FeatureFlags = new FeatureFlags
            {
                HasPriority = true
            },
            Attributes = new Attributes
            {
                NumberOfBumps = 1,
                NumberOfLocations = 1,
                Duration = TimeSpan.FromDays(30)
            },
            Prices = [
                new Price(Guid.NewGuid(), new DateTimeRange(), DefaultPrice)
            ]
        };
        
        var product = new Product(@event);
        
        product.Id.Should().Be(@event.ProductId);
        product.Name.Should().Be(@event.Name);
        product.Description.Should().Be(@event.Description);
        product.Attributes.Should().Be(@event.Attributes);
        product.FeatureFlags.Should().Be(@event.FeatureFlags);
        product.IsActive.Should().Be(@event.IsActive);
        product.Prices.Should().HaveCount(1);
        product.Prices[0].Id.Should().Be(@event.Prices[0].Id);
        product.Prices[0].Money.Amount.Should().Be(@event.Prices[0].Money.Amount);
        product.Prices[0].DateTimeRange.Should().Be(@event.Prices[0].DateTimeRange);
    }
    
    [Fact]
    public void ShouldUpdateProduct_WhenProductNameChangedEventIsApplied()
    {
        // Arrange
        var product = AnExistingProduct();

        var @event = new ProductNameChanged("New Name");
        
        // Act
        product.Apply(@event);

        // Assert
        product.Name.Should().Be(@event.Name);
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

        var @event = new ProductPriceCreated(Guid.NewGuid(), new DateTimeRange(DateTimeOffset.UtcNow.AddDays(100), null), new Money(100, Currency.EUR));
        
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

        var @event = new ProductPriceChanged(FuturePriceId, new DateTimeRange(DateTimeOffset.UtcNow.AddDays(100), null), new Money(100, Currency.EUR));
        
        // Act
        product.Apply(@event);

        // Assert
        product.Prices.Should().Contain(p => p.Id == @event.Id
            && p.Money.Amount == @event.Money.Amount
            && p.DateTimeRange == @event.DateTimeRange);
    }
    
    [Fact]
    public void ShouldDeletePrice_WhenPriceRemovedEventIsApplied()
    {
        // Arrange
        var product = AnExistingProduct();

        var @event = new ProductPriceRemoved(FuturePriceId, new DateTimeRange(DateTimeOffset.UtcNow.AddDays(100), null), new Money(100, Currency.EUR));
        
        // Act
        product.Apply(@event);

        // Assert
        product.Prices.Should().NotContain(p => p.Id == @event.Id);
    }
    
    [Fact]
    public void ShouldGetPrice_WhenPriceForTimestampIsRequested()
    {
        // Arrange
        var product = AnExistingProduct(isActive: true);
        var now = DateTimeOffset.UtcNow;
        
        // Act
        var currentPrice = product.GetPrice(now);
        var futurePrice = product.GetPrice(now.AddMonths(1));
        
        // Assert
        currentPrice.Id.Should().Be(DefaultPriceId);
        currentPrice.Money.Amount.Should().Be(DefaultPriceAmount);
        
        futurePrice.Id.Should().Be(FuturePriceId);
        futurePrice.Money.Should().Be(FuturePrice);
    }
    
    [Fact]
    public void ShouldThrowException_WhenPricesHaveOverlappingDateRanges()
    {
        // Arrange
        var product = AnExistingProduct();
        var prices = new List<Price>
        {
            new(DefaultPriceId, new DateTimeRange(), new Money(99, Currency.EUR)),
            new(Guid.NewGuid(), new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(-1), null), new Money(100, Currency.EUR)),
            new(Guid.NewGuid(), new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(-10), DateTimeOffset.UtcNow.AddDays(10)), new Money(100, Currency.EUR))
        };
        
        // Act
        var act = () => product.Update(product.Name, product.Description, true, product.FeatureFlags, product.Attributes, prices);
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    private static Product AnExistingProduct(bool isActive = false)
    {
        var product = new Product("Test Product", "Desc", isActive, 
        new FeatureFlags
        {
            HasPriority = true
        },
        new Attributes
        {
          NumberOfBumps  = 1,
          NumberOfLocations = 1,
          Duration = TimeSpan.FromDays(1)
        },
        [
            new Price(DefaultPriceId, new DateTimeRange(), DefaultPrice),
            new Price(FuturePriceId, new DateTimeRange(DateTimeOffset.UtcNow.Date.AddDays(10), null), FuturePrice),
        ]);

        _ = product.DequeueEvents().ToList();
        
        return product;
    }
    

}