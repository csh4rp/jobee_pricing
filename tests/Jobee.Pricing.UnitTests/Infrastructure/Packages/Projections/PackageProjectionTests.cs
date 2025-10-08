using JasperFx.Events;
using JasperFx.Events.Daemon;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Infrastructure.Packages.Projections;
using NSubstitute;
using AwesomeAssertions;

namespace Jobee.Pricing.UnitTests.Infrastructure.Packages.Projections;

public class PackageProjectionTests
{
    private static IEvent<T> BuildEvent<T>(T data, Guid streamId, DateTimeOffset? timestamp = null) where T : notnull
    {
        var ev = Substitute.For<IEvent<T>>();
        ev.Data.Returns(data);
        ev.StreamId.Returns(streamId);
        ev.Timestamp.Returns(timestamp ?? DateTimeOffset.UtcNow);
        return ev;
    }

    [Fact]
    public void ShouldReturnNothing_WhenNoCreatedEventOnNewStream()
    {
        var projection = new PackageProjection();
        var (snapshot, action) = projection.DetermineAction(null, Guid.NewGuid(), []);
        action.Should().Be(ActionType.Nothing);
        snapshot.Should().BeNull();
    }

    [Fact]
    public void ShouldCreateSnapshot_WhenOnPackageCreatedEventIsDispatched()
    {
        var projection = new PackageProjection();
        var packageId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var prices = new List<Price>
        {
            new(Guid.NewGuid(), new DateTimeRange(), new Money(10, Currency.PLN))
        };

        var @event = new PackageCreated
        {
            PackageId = packageId,
            ProductId = productId,
            Name = "Pkg-1",
            Description = "Desc",
            IsActive = true,
            Quantity = 3,
            Prices = prices
        };
        var eventWrapper = BuildEvent(@event, packageId);

        var (snapshot, action) = projection.DetermineAction(null, packageId, new List<IEvent> { eventWrapper });

        action.Should().Be(ActionType.Store);
        snapshot.Should().NotBeNull();
        snapshot.Id.Should().Be(packageId);
        snapshot.ProductId.Should().Be(productId);
        snapshot.Name.Should().Be(@event.Name);
        snapshot.Description.Should().Be(@event.Description);
        snapshot.IsActive.Should().BeTrue();
        snapshot.Quantity.Should().Be(@event.Quantity);
        snapshot.Prices.Should().HaveCount(@event.Prices.Count);
        snapshot.CreatedAt.Should().Be(eventWrapper.Timestamp);
        snapshot.LastModifiedAt.Should().BeNull();
    }

    [Fact]
    public void ShouldUpdateFields_OnNameAndDescriptionAndActivationChanges()
    {
        var projection = new PackageProjection();
        var packageId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var created = BuildEvent(new PackageCreated
        {
            PackageId = packageId,
            ProductId = productId,
            Name = "Pkg-1",
            Description = "Desc",
            IsActive = false,
            Quantity = 1,
            Prices = new List<Price>()
        }, packageId, DateTimeOffset.UtcNow.AddMinutes(-10));

        var (snapshot, _) = projection.DetermineAction(null, packageId, new List<IEvent> { (IEvent)created });
        snapshot!.LastModifiedAt.Should().BeNull();

        var nameChanged = BuildEvent(new PackageNameChanged("New Name"), packageId, DateTimeOffset.UtcNow.AddMinutes(-9));
        var descriptionChanged = BuildEvent(new PackageDescriptionChanged("New Desc"), packageId, DateTimeOffset.UtcNow.AddMinutes(-8));
        var activated = BuildEvent(new PackageActivated(), packageId, DateTimeOffset.UtcNow.AddMinutes(-7));
        var deactivated = BuildEvent(new PackageDeactivated(), packageId, DateTimeOffset.UtcNow.AddMinutes(-6));

        (snapshot, _) = projection.DetermineAction(snapshot, packageId, new List<IEvent>
        {
            nameChanged,
            descriptionChanged,
            activated,
            deactivated
        });

        snapshot.Should().NotBeNull();
        snapshot.Name.Should().Be("New Name");
        snapshot.Description.Should().Be("New Desc");
        snapshot.IsActive.Should().BeFalse(); // deactivated last
        snapshot.LastModifiedAt.Should().Be(deactivated.Timestamp);
    }

    [Fact]
    public void ShouldHandlePriceLifecycle()
    {
        var projection = new PackageProjection();
        var packageId = Guid.NewGuid();
        var created = BuildEvent(new PackageCreated
        {
            PackageId = packageId,
            ProductId = Guid.NewGuid(),
            Name = "Pkg-1",
            Description = "Desc",
            IsActive = true,
            Quantity = 1,
            Prices = new List<Price>()
        }, packageId);

        var (snapshot, _) = projection.DetermineAction(null, packageId, new List<IEvent> { (IEvent)created });

        var priceId = Guid.NewGuid();
        var priceCreated = BuildEvent(new PackagePriceCreated(priceId, new DateTimeRange(), new Money(10, Currency.PLN)), packageId, DateTimeOffset.UtcNow.AddMinutes(1));
        (snapshot, _) = projection.DetermineAction(snapshot, packageId, new List<IEvent> { (IEvent)priceCreated });

        snapshot!.Prices.Should().HaveCount(1);
        snapshot.Prices[0].Money.Amount.Should().Be(10);

        var priceChanged = BuildEvent(new PackagePriceChanged(priceId, new DateTimeRange(), new Money(12, Currency.PLN)), packageId, DateTimeOffset.UtcNow.AddMinutes(2));
        (snapshot, _) = projection.DetermineAction(snapshot, packageId, new List<IEvent> { (IEvent)priceChanged });
        snapshot.Prices[0].Money.Amount.Should().Be(12);

        var priceRemoved = BuildEvent(new PackagePriceRemoved(priceId, new DateTimeRange(), new Money(12, Currency.PLN)), packageId, DateTimeOffset.UtcNow.AddMinutes(3));
        (snapshot, _) = projection.DetermineAction(snapshot, packageId, new List<IEvent> { (IEvent)priceRemoved });
        snapshot.Prices.Should().BeEmpty();
    }

    [Fact]
    public void ShouldUpdateProductName_WhenMatchingProductStreamEvent()
    {
        var projection = new PackageProjection();
        var packageId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var created = BuildEvent(new PackageCreated
        {
            PackageId = packageId,
            ProductId = productId,
            Name = "Pkg-1",
            Description = "Desc",
            IsActive = true,
            Quantity = 1,
            Prices = new List<Price>()
        }, packageId);

        var (snapshot, _) = projection.DetermineAction(null, packageId, new List<IEvent> { (IEvent)created });
        snapshot!.ProductName.Should().BeEmpty();

        var productNameChangedDifferent = BuildEvent(new ProductNameChanged("Other Product"), Guid.NewGuid());
        (snapshot, _) = projection.DetermineAction(snapshot, packageId, new List<IEvent> { (IEvent)productNameChangedDifferent });
        snapshot.ProductName.Should().BeEmpty();

        var productNameChanged = BuildEvent(new ProductNameChanged("Main Product"), productId);
        (snapshot, _) = projection.DetermineAction(snapshot, packageId, new List<IEvent> { (IEvent)productNameChanged });
        snapshot.ProductName.Should().Be("Main Product");
        snapshot.LastModifiedAt.Should().BeNull(); // product name change does not modify LastModifiedAt
    }

    [Fact]
    public void ShouldDeleteSnapshot_OnArchive()
    {
        var projection = new PackageProjection();
        var packageId = Guid.NewGuid();
        var created = BuildEvent(new PackageCreated
        {
            PackageId = packageId,
            ProductId = Guid.NewGuid(),
            Name = "Pkg-1",
            Description = "Desc",
            IsActive = true,
            Quantity = 1,
            Prices = new List<Price>()
        }, packageId);

        var (snapshot, _) = projection.DetermineAction(null, packageId, new List<IEvent> { (IEvent)created });

        var archived = BuildEvent(new PackageArchived(), packageId);
        var (deletedSnapshot, action) = projection.DetermineAction(snapshot, packageId, new List<IEvent> { (IEvent)archived });

        action.Should().Be(ActionType.Delete);
        deletedSnapshot.Should().BeNull();
    }
}
