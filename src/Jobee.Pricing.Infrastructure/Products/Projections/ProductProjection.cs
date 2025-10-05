using JasperFx.Events;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Events;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Marten.Events.Aggregation;

namespace Jobee.Pricing.Infrastructure.Products.Projections;

public class ProductProjection : SingleStreamProjection<ProductProjectionModel, Guid>
{
    public ProductProjection()
    {
        Name = "Product";
    }
    
    public void Apply(IEvent<ProductNameChanged> @event, ProductProjectionModel model)
    {
        model.Name = @event.Data.Name;
        model.LastModifiedAt = @event.Timestamp;
    }
    
    public void Apply(IEvent<ProductActivated> @event, ProductProjectionModel model)
    {
        model.IsActive = true;
        model.LastModifiedAt = @event.Timestamp;
    }
    
    public void Apply(IEvent<ProductDeactivated> @event, ProductProjectionModel model)
    {
        model.IsActive = false;
        model.LastModifiedAt = @event.Timestamp;
    }
    
    public void Apply(IEvent<PackagePriceChanged> @event, ProductProjectionModel model)
    {
        model.LastModifiedAt = @event.Timestamp;
        var index = model.Prices.FindIndex(p => p.Id == @event.Data.Id);
        model.Prices[index] = new Price(
            @event.Data.Id,
            @event.Data.DateTimeRange,
            @event.Data.Money);
    }
    
    public void Apply(IEvent<PackagePriceCreated> @event, ProductProjectionModel model)
    {
        model.LastModifiedAt = @event.Timestamp;
        model.Prices.Add(new Price(
            @event.Data.Id,
            @event.Data.DateTimeRange,
            @event.Data.Money));
    }
    
    public void Apply(IEvent<PackagePriceRemoved> @event, ProductProjectionModel model)
    {
        model.LastModifiedAt = @event.Timestamp;
        var index = model.Prices.FindIndex(p => p.Id == @event.Data.Id);
        model.Prices.RemoveAt(index);
    }
    
    public ProductProjectionModel Create(IEvent<ProductCreated> @event)
    {
        return new ProductProjectionModel
        {
            Id = @event.Id,
            Name = @event.Data.Name,
            IsActive = @event.Data.IsActive,
            CreatedAt = @event.Timestamp,
            LastModifiedAt = null,
            Prices = [.. @event.Data.Prices]
        };
    }
}