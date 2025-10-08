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

public class PackageProjection : MultiStreamProjection<PackageProjectionModel, Guid>
{
    public PackageProjection()
    {
        Name = "Package";
    }

    public override (PackageProjectionModel?, ActionType) DetermineAction(PackageProjectionModel? snapshot,
        Guid identity,
        IReadOnlyList<IEvent> events)
    {
        if (snapshot is null)
        {
            var createdEvent = events.OfType<IEvent<PackageCreated>>().FirstOrDefault();
            if (createdEvent is not null)
            {
                var model = new PackageProjectionModel
                {
                    Id = createdEvent.StreamId,
                    ProductId = createdEvent.Data.ProductId,
                    Name = createdEvent.Data.Name,
                    Description = createdEvent.Data.Description,
                    IsActive = createdEvent.Data.IsActive,
                    CreatedAt = createdEvent.Timestamp,
                };
                return (model, ActionType.Store);
            }

            return (null, ActionType.Nothing);
        }

        foreach (var @event in events)
        {
            switch (@event)
            {
                case IEvent<PackageNameChanged> nameChanged:
                    snapshot.Name = nameChanged.Data.Name;
                    snapshot.LastModifiedAt = nameChanged.Timestamp;
                    break;
                case IEvent<PackageDescriptionChanged> descriptionChanged:
                    snapshot.Description = descriptionChanged.Data.Description;
                    snapshot.LastModifiedAt = descriptionChanged.Timestamp;
                    break;
                case IEvent<PackageActivated> activated:
                    snapshot.IsActive = true;
                    snapshot.LastModifiedAt = activated.Timestamp;
                    break;
                case IEvent<PackageDeactivated> deactivated:
                    snapshot.IsActive = false;
                    snapshot.LastModifiedAt = deactivated.Timestamp;
                    break;
                case IEvent<PackagePriceCreated> priceCreated:
                    snapshot.Prices.Add(new Price(priceCreated.Data.Id, priceCreated.Data.DateTimeRange,
                        priceCreated.Data.Money));
                    snapshot.LastModifiedAt = priceCreated.Timestamp;
                    break;
                case IEvent<PackagePriceChanged> priceChanged:
                    var index = snapshot.Prices.FindIndex(p => p.Id == priceChanged.Data.Id);
                    if (index != -1)
                    {
                        snapshot.Prices[index] = new Price(priceChanged.Data.Id, priceChanged.Data.DateTimeRange,
                            priceChanged.Data.Money);
                        snapshot.LastModifiedAt = priceChanged.Timestamp;
                    }

                    break;
                case IEvent<PackagePriceRemoved> priceRemoved:
                    var removeIndex = snapshot.Prices.FindIndex(p => p.Id == priceRemoved.Data.Id);
                    if (removeIndex != -1)
                    {
                        snapshot.Prices.RemoveAt(removeIndex);
                        snapshot.LastModifiedAt = priceRemoved.Timestamp;
                    }

                    break;
                case IEvent<ProductNameChanged> productNameChanged:
                    if (productNameChanged.StreamId == snapshot.ProductId)
                    {
                        snapshot.ProductName = productNameChanged.Data.Name;
                    }

                    break;
                case IEvent<PackageArchived> archived:
                    return (null, ActionType.Delete);
                default:
                    break;
            }
        }

        return (snapshot, ActionType.Store);
    }
}