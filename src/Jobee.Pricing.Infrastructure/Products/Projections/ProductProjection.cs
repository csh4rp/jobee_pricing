using JasperFx.Events;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Events;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Marten.Events.Aggregation;

namespace Jobee.Pricing.Infrastructure.Products.Projections;

public class ProductProjection : SingleStreamProjection<ProductProjection, Guid>
{
    public Guid ProductId { get; init; }

    public string ProductName { get; set; } = null!;
    
    public string ProductDescription { get; set; } = null!;

    public bool IsActive { get; set; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? LastModifiedAt { get; set; }
    
    public List<Price> Prices { get; set; } = [];
    
    public ProductProjection()
    {
        Name = "Product";
    }
    
    public static void Apply(IEvent<ProductNameChanged> @event, ProductProjection model)
    {
        model.ProductName = @event.Data.Name;
        model.LastModifiedAt = @event.Timestamp;
    }

    public static void Apply(IEvent<ProductDescriptionChanged> @event, ProductProjection model)
    {
        model.ProductDescription = @event.Data.Description;
        model.LastModifiedAt = @event.Timestamp;
    }

    
    public static void Apply(IEvent<ProductActivated> @event, ProductProjection model)
    {
        model.IsActive = true;
        model.LastModifiedAt = @event.Timestamp;
    }
    
    public static void Apply(IEvent<ProductDeactivated> @event, ProductProjection model)
    {
        model.IsActive = false;
        model.LastModifiedAt = @event.Timestamp;
    }
    
    public static void Apply(IEvent<ProductPriceChanged> @event, ProductProjection model)
    {
        model.LastModifiedAt = @event.Timestamp;
        var index = model.Prices.FindIndex(p => p.Id == @event.Data.Id);
        
        if (index == -1)
        {
            return;
        }
        
        model.Prices[index] = new Price(
            @event.Data.Id,
            @event.Data.DateTimeRange,
            @event.Data.Value);
    }
    
    public static void Apply(IEvent<ProductPriceCreated> @event, ProductProjection model)
    {
        model.LastModifiedAt = @event.Timestamp;
        model.Prices.Add(new Price(
            @event.Data.Id,
            @event.Data.DateTimeRange,
            @event.Data.Price));
    }
    
    public static void Apply(IEvent<ProductPriceRemoved> @event, ProductProjection model)
    {
        model.LastModifiedAt = @event.Timestamp;
        var index = model.Prices.FindIndex(p => p.Id == @event.Data.Id);
        model.Prices.RemoveAt(index);
    }
    
    public static ProductProjection Create(IEvent<ProductCreated> @event)
    {
        return new ProductProjection
        {
            ProductId = @event.Data.Id,
            ProductName = @event.Data.Name,
            ProductDescription = @event.Data.Description,
            IsActive = @event.Data.IsActive,
            CreatedAt = @event.Timestamp,
            LastModifiedAt = null,
            Prices = [.. @event.Data.Prices]
        };
    }
}