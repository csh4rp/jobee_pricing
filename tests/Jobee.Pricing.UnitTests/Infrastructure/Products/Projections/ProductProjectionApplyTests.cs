using AwesomeAssertions;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Infrastructure.Products.Projections;
using JasperFx.Events;
using Jobee.Pricing.Domain.Events;

namespace Jobee.Pricing.UnitTests.Infrastructure.Products.Projections;

public class ProductProjectionApplyTests
{
    
    [Fact]
    public void ShouldCreateProjection_WhenApplyProductCreated()
    {
        var productId = Guid.NewGuid();
        var @event = new ProductCreated
        {
            Id = productId,
            Name = "Prod-1",
            Description = "Desc",
            IsActive = true,
            Prices = [new Price(Guid.NewGuid(), new DateTimeRange(), new Money(10, Currency.EUR))],
            FeatureFlags = new FeatureFlags
            {
                HasPriority = true
            },
            Attributes = new Attributes
            {
                Duration = TimeSpan.FromHours(1),
                NumberOfBumps = 2,
                NumberOfLocations = 10
            }
        };
        var wrappedEvent = CreateWrappedEvent(@event);
        
        var snapshot = ProductProjection.Create(wrappedEvent);
        
        snapshot.ProductId.Should().Be(@event.Id);
        snapshot.ProductName.Should().Be(@event.Name);
        snapshot.ProductDescription.Should().Be(@event.Description);
        snapshot.IsActive.Should().BeTrue();
        snapshot.Prices.Should().HaveCount(1);
        snapshot.Prices[0].Id.Should().Be(@event.Prices[0].Id);
        snapshot.Prices[0].DateTimeRange.Should().Be(@event.Prices[0].DateTimeRange);
        snapshot.Prices[0].Value.Should().Be(@event.Prices[0].Value);
        
        snapshot.CreatedAt.Should().Be(wrappedEvent.Timestamp);
        snapshot.LastModifiedAt.Should().BeNull();
    }
    
    [Fact]
    public void ShouldUpdateName_WhenApplyProductNameChanged()
    {
        var snapshot = new ProductProjection { ProductName = "Old" };
        var @event = new ProductNameChanged("NewName");
        var wrappedEvent = CreateWrappedEvent(@event);
        
        ProductProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.ProductName.Should().Be(@event.Name);
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }
    
    [Fact]
    public void ShouldUpdateDescription_WhenApplyProductDescriptionChanged()
    {
        var snapshot = new ProductProjection { ProductDescription = "Old" };
        var @event = new ProductDescriptionChanged("NewDescription");
        var wrappedEvent = CreateWrappedEvent(@event);
        
        ProductProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.ProductDescription.Should().Be(@event.Description);
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldSetIsActiveTrue_WhenApplyProductActivated()
    {
        var snapshot = new ProductProjection { IsActive = false };
        var wrappedEvent = CreateWrappedEvent(new ProductActivated());
        
        ProductProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.IsActive.Should().BeTrue();
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldSetIsActiveFalse_WhenApplyProductDeactivated()
    {
        var snapshot = new ProductProjection { IsActive = true };
        var wrappedEvent = CreateWrappedEvent(new ProductDeactivated());
        
        ProductProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.IsActive.Should().BeFalse();
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldUpdatePrice_WhenApplyProductPriceChanged()
    {
        var priceId = Guid.NewGuid();
        var snapshot = new ProductProjection
        {
            Prices = [new(priceId, new DateTimeRange(), new Money(5, Currency.PLN))]
        };

        var @event = new ProductPriceChanged(priceId, new DateTimeRange(), new Money(15, Currency.PLN));
        var wrappedEvent = CreateWrappedEvent(@event);
        
        ProductProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.Prices.Should().ContainSingle(p => p.Id == priceId && p.Value.Amount == 15);
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldAddPrice_WhenApplyProductPriceCreated()
    {
        var snapshot = new ProductProjection { Prices = [] };
        var priceId = Guid.NewGuid();
        var @event = new ProductPriceCreated(priceId, new DateTimeRange(), new Money(10, Currency.PLN));
        var wrappedEvent = CreateWrappedEvent(@event);
        
        ProductProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.Prices.Should().ContainSingle(p => p.Id == priceId && p.Value == @event.Price);
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }
    
    private static IEvent<T> CreateWrappedEvent<T>(T data) where T : notnull =>
        new Event<T>(data)
        {
            Id = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero)
        };
}
