using JasperFx.Events;
using Jobee.Pricing.Domain.Events;
using Jobee.Pricing.Infrastructure.DataAccess.Models;
using Marten.Events.Aggregation;

namespace Jobee.Pricing.Infrastructure.DataAccess;

public class ProductProjection : SingleStreamProjection<ProductProjectionModel, Guid>
{
    public ProductProjection()
    {
        Name = "Product";
    }
    
    public void Apply(IEvent<ProductChanged> @event, ProductProjectionModel model)
    {
        model.Name = @event.Data.Name;
        model.NumberOfOffers = @event.Data.NumberOfOffers;
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
            Amount = @event.Data.Amount,
            StartsAt = @event.Data.DateRange.StartsAt,
            EndsAt = @event.Data.DateRange.EndsAt
        };
    }
    
    public void Apply(IEvent<PriceCreated> @event, ProductProjectionModel model)
    {
        model.LastModifiedAt = @event.Timestamp;
        model.Prices.Add(new PriceProjectionModel
        {
            Id = @event.Data.Id,
            Amount = @event.Data.Amount,
            StartsAt = @event.Data.DateRange.StartsAt,
            EndsAt = @event.Data.DateRange.EndsAt
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
            Id = @event.Data.Id,
            Name = @event.Data.Name,
            NumberOfOffers = @event.Data.NumberOfOffers,
            IsActive = @event.Data.IsActive,
            CreatedAt = @event.Timestamp,
            LastModifiedAt = null,
            Prices = @event.Data.Prices.Select(price => new PriceProjectionModel
            {
                Id = price.Id,
                Amount = price.Amount,
                StartsAt = price.DateRange.StartsAt,
                EndsAt = price.DateRange.EndsAt
            }).ToList()
        };
    }
}