using JasperFx.Events;
using JasperFx.Events.Daemon;
using JasperFx.Events.Grouping;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Marten;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace Jobee.Pricing.Infrastructure.Packages.Projections;

public class PackageProjection : SingleStreamProjection<PackageProjection, Guid>
{
    public Guid PackageId { get; set; }

    public Guid ProductId { get; set; }

    public string PackageName { get; set; } = null!;

    public string PackageDescription { get; set; } = null!;

    public bool IsActive { get; set; }
    
    public int Quantity { get; set; }
    
    public List<Price> Prices { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? LastModifiedAt { get; set; }
    
    public PackageProjection()
    {
        Name = "Package";
    }

    public static void Apply(IEvent<PackageNameChanged> @event, PackageProjection snapshot)
    {
        snapshot.PackageName = @event.Data.Name;
        snapshot.LastModifiedAt = @event.Timestamp;
    }

    public static void Apply(IEvent<PackageDescriptionChanged> @event, PackageProjection snapshot)
    {
        snapshot.PackageDescription = @event.Data.Description;
        snapshot.LastModifiedAt = @event.Timestamp;
    }

    public static void Apply(IEvent<PackageActivated> @event, PackageProjection snapshot)
    {
        snapshot.IsActive = true;
        snapshot.LastModifiedAt = @event.Timestamp;
    }

    public static void Apply(IEvent<PackageDeactivated> @event, PackageProjection snapshot)
    {
        snapshot.IsActive = false;
        snapshot.LastModifiedAt = @event.Timestamp;
    }

    public static void Apply(IEvent<PackagePriceCreated> @event, PackageProjection snapshot)
    {
        snapshot.Prices.Add(new Price(@event.Data.Id, @event.Data.DateTimeRange, @event.Data.Value));
        snapshot.LastModifiedAt = @event.Timestamp;
    }

    public static void Apply(IEvent<PackagePriceChanged> @event, PackageProjection snapshot)
    {
        var index = snapshot.Prices.FindIndex(p => p.Id == @event.Data.Id);
        if (index != -1)
        {
            snapshot.Prices[index] = new Price(@event.Data.Id, @event.Data.DateTimeRange, @event.Data.Value);
            snapshot.LastModifiedAt = @event.Timestamp;
        }
    }

    public static void Apply(IEvent<PackagePriceRemoved> @event, PackageProjection snapshot)
    {
        var index = snapshot.Prices.FindIndex(p => p.Id == @event.Data.Id);
        if (index != -1)
        {
            snapshot.Prices.RemoveAt(index);
            snapshot.LastModifiedAt = @event.Timestamp;
        }
    }
    public static PackageProjection Create(IEvent<PackageCreated> packageCreated)
    {
        return new PackageProjection
        {
            PackageId = packageCreated.Data.Id,
            ProductId = packageCreated.Data.ProductId,
            PackageName = packageCreated.Data.Name,
            PackageDescription = packageCreated.Data.Description,
            IsActive = packageCreated.Data.IsActive,
            Quantity = packageCreated.Data.Quantity,
            Prices = [.. packageCreated.Data.Prices],
            CreatedAt = packageCreated.Timestamp,
            LastModifiedAt = null,
        };
    }
}