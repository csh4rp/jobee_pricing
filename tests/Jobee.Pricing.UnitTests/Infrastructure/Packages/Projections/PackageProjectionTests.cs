using AwesomeAssertions;
using JasperFx.Events;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Infrastructure.Packages.Projections;

namespace Jobee.Pricing.UnitTests.Infrastructure.Packages.Projections;

public class PackageProjectionApplyTests
{
    [Fact]
    public void ShouldInitializeSnapshot_WhenApplyPackageCreated()
    {
        var packageId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var prices = new List<Price> { new(Guid.NewGuid(), new DateTimeRange(), new Money(10, Currency.PLN)) };
        var @event = new PackageCreated
        {
            Id = packageId,
            ProductId = productId,
            Name = "Pkg-1",
            Description = "Desc",
            IsActive = true,
            Quantity = 3,
            Prices = prices
        };
        
        var wrappedEvent = CreateWrappedEvent(@event);
        
        var snapshot = PackageProjection.Create(wrappedEvent);
        
        snapshot.PackageId.Should().Be(packageId);
        snapshot.ProductId.Should().Be(productId);
        snapshot.PackageName.Should().Be(@event.Name);
        snapshot.PackageDescription.Should().Be(@event.Description);
        snapshot.IsActive.Should().BeTrue();
        snapshot.Quantity.Should().Be(@event.Quantity);
        snapshot.Prices.Should().HaveCount(prices.Count);
        snapshot.Prices[0].Id.Should().Be(@event.Prices[0].Id);
        snapshot.Prices[0].DateTimeRange.Should().Be(@event.Prices[0].DateTimeRange);
        snapshot.Prices[0].Value.Should().Be(@event.Prices[0].Value);
        snapshot.CreatedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldUpdateName_WhenApplyPackageNameChanged()
    {
        var snapshot = new PackageProjection { PackageName = "Old" };
        var @event = new PackageNameChanged("NewName");
        var wrappedEvent = CreateWrappedEvent(@event);
        
        PackageProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.PackageName.Should().Be(@event.Name);
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldUpdateDescription_WhenApplyPackageDescriptionChanged()
    {
        var snapshot = new PackageProjection { PackageDescription = "OldDesc" };
        var @event = new PackageDescriptionChanged("NewDesc");
        var wrappedEvent = CreateWrappedEvent(@event);
        
        PackageProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.PackageDescription.Should().Be(@event.Description);
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldSetIsActiveTrue_WhenApplyPackageActivated()
    {
        var snapshot = new PackageProjection { IsActive = false };
        var @event = new PackageActivated();
        var wrappedEvent = CreateWrappedEvent(@event);
        
        PackageProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.IsActive.Should().BeTrue();
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldSetIsActiveFalse_WhenApplyPackageDeactivated()
    {
        var snapshot = new PackageProjection { IsActive = true };
        var @event = new PackageDeactivated();
        var wrappedEvent = CreateWrappedEvent(@event);
        
        PackageProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.IsActive.Should().BeFalse();
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldAddPrice_WhenApplyPackagePriceCreated()
    {
        var snapshot = new PackageProjection { Prices = new List<Price>() };
        var priceId = Guid.NewGuid();
        var @event = new PackagePriceCreated(priceId, new DateTimeRange(), new Money(10, Currency.PLN));
        var wrappedEvent = CreateWrappedEvent(@event);
        
        PackageProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.Prices.Should().ContainSingle(p => p.Id == priceId 
                                                    && p.DateTimeRange == @event.DateTimeRange
                                                    && p.Value == @event.Value);
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldUpdatePrice_WhenApplyPackagePriceChanged()
    {
        var priceId = Guid.NewGuid();
        var snapshot = new PackageProjection
        {
            Prices = [new(priceId, new DateTimeRange(), new Money(5, Currency.PLN))]
        };
        var @event = new PackagePriceChanged(priceId, new DateTimeRange(), new Money(15, Currency.PLN));
        var wrappedEvent = CreateWrappedEvent(@event);
        
        PackageProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.Prices.Should().ContainSingle(p => p.Id == priceId 
                                                    && p.DateTimeRange == @event.DateTimeRange
                                                    && p.Value == @event.Value);
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    [Fact]
    public void ShouldRemovePrice_WhenApplyPackagePriceRemoved()
    {
        var priceId = Guid.NewGuid();
        var snapshot = new PackageProjection
        {
            Prices = [new(priceId, new DateTimeRange(), new Money(5, Currency.PLN))]
        };
        var @event = new PackagePriceRemoved(priceId, new DateTimeRange(), new Money(5, Currency.PLN));
        var wrappedEvent = CreateWrappedEvent(@event);
        
        PackageProjection.Apply(wrappedEvent, snapshot);
        
        snapshot.Prices.Should().BeEmpty();
        snapshot.LastModifiedAt.Should().Be(wrappedEvent.Timestamp);
    }

    private static Event<T> CreateWrappedEvent<T>(T data) where T : notnull => new(data)
    {
        Id = Guid.NewGuid(),
        Timestamp = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero)
    };
}
