using JasperFx.Events;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Events;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Infrastructure.Products.Models;
using Marten.Events.Aggregation;

namespace Jobee.Pricing.Infrastructure.Products;

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
    
    public void Apply(IEvent<PriceChanged> @event, ProductProjectionModel model)
    {
        model.LastModifiedAt = @event.Timestamp;
        var index = model.Prices.FindIndex(p => p.Id == @event.Data.Id);
        model.Prices[index] = new  PriceProjectionModel
        {
            Id = @event.Data.Id,
            Money = @event.Data.Money,
            StartsAt = @event.Data.DateTimeRange.StartsAt,
            EndsAt = @event.Data.DateTimeRange.EndsAt
        };
    }
    
    public void Apply(IEvent<PriceCreated> @event, ProductProjectionModel model)
    {
        model.LastModifiedAt = @event.Timestamp;
        model.Prices.Add(new PriceProjectionModel
        {
            Id = @event.Data.Id,
            Money = @event.Data.Money,
            StartsAt = @event.Data.DateTimeRange.StartsAt,
            EndsAt = @event.Data.DateTimeRange.EndsAt
        });
    }
    
    public void Apply(IEvent<PriceRemoved> @event, ProductProjectionModel model)
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
            Prices = [.. @event.Data.Prices.Select(price => new PriceProjectionModel
            {
                Id = price.Id,
                Money = price.Money,
                StartsAt = price.DateTimeRange.StartsAt,
                EndsAt = price.DateTimeRange.EndsAt
            })]
        };
    }
}