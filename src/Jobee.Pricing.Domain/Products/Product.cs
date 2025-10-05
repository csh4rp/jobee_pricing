using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Events;
using Jobee.Pricing.Domain.Packages;

namespace Jobee.Pricing.Domain.Products;

public class Product : Entity<Guid>
{
    private readonly List<Price> _prices;
    
    public bool IsActive { get; private set; }

    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public FeatureFlags FeatureFlags { get; private set; }
    
    public Attributes Attributes { get; private set; }
    
    public IReadOnlyList<Price> Prices => _prices;
    
    public Product(ProductCreated @event)
    {
        Id = @event.ProductId;
        IsActive = @event.IsActive;
        Name = @event.Name;
        Description = @event.Description;
        FeatureFlags = @event.FeatureFlags;
        Attributes = @event.Attributes;
        _prices = [.. @event.Prices.Select(p => new Price(p.Id, p.DateTimeRange, p.Money))];
    }
    
    public Product(string name, string description, bool isActive,
        FeatureFlags featureFlags, Attributes attributes,
        IReadOnlyList<Price> prices)
    {
        PriceValidator.ValidatePrices(prices);
        
        Id = Guid.CreateVersion7();
        IsActive = isActive;
        Name = name;
        Description = description;
        FeatureFlags = featureFlags;
        Attributes = attributes;
        _prices = prices as List<Price> ?? [.. prices];
        EnqueueEvent(new ProductCreated
        {
            ProductId = Id,
            Name = Name,
            Description = Description,
            IsActive = IsActive,
            FeatureFlags = FeatureFlags,
            Attributes = Attributes,
            Prices = [.. Prices]
        });
    }
    
    public void Update(string name, string description, bool isActive,
        FeatureFlags featureFlags, Attributes attributes,
        IReadOnlyCollection<Price> prices)
    {
        PriceValidator.ValidatePrices(prices);
        
        if (name != Name)
        {
            Name = name;
            EnqueueEvent(new ProductNameChanged(name));
        }

        if (description != Description)
        {
            Description = description;
            EnqueueEvent(new ProductDescriptionChanged(description));
        }

        if (isActive != IsActive)
        {
            IsActive = isActive;
            EnqueueEvent(isActive ? new ProductActivated() : new ProductDeactivated());
        }
        
        if (featureFlags != FeatureFlags)
        {
            FeatureFlags = featureFlags;
            EnqueueEvent(new ProductFeatureFlagsChanged(featureFlags));
        }
        
        if (attributes != Attributes)
        {
            Attributes = attributes;
            EnqueueEvent(new ProductAttributesChanged(attributes));
        }

        foreach (var price in prices)
        {
            var existingPrice = _prices.FirstOrDefault(p => p.Id == price.Id);
            if (existingPrice is not null && !existingPrice.Equals(price))
            {
                EnqueueEvent(new ProductPriceChanged(existingPrice.Id, existingPrice.DateTimeRange, price.Money));
            }
            else if (existingPrice is null)
            {
                EnqueueEvent(new ProductPriceCreated(price.Id, price.DateTimeRange, price.Money));
            }
        }

        foreach (var price in _prices)
        {
            var isPriceRemoved = prices.All(p => p.Id != price.Id);
            if (isPriceRemoved)
            {
                EnqueueEvent(new ProductPriceRemoved(price.Id, price.DateTimeRange, price.Money));
            }
        }
    }
    public Price GetPrice(DateTimeOffset timestamp)
    {
        var price = _prices.Where(p => 
            p.DateTimeRange.Overlaps(timestamp))
            .OrderBy(t => t.IsDefault ? 1 : 0)
            .First();

        return price;
    }

    public void Apply(ProductNameChanged @event)
    {
        Name = @event.Name;
    }
    
    public void Apply(ProductDescriptionChanged @event)
    {
        Description = @event.Description;
    }
    
    public void Apply(ProductFeatureFlagsChanged @event)
    {
        FeatureFlags = new FeatureFlags
        {
            HasPriority = @event.HasPriority
        };
    }
    
    public void Apply(ProductAttributesChanged @event)
    {
        Attributes = new Attributes
        {
            Duration = @event.Duration,
            NumberOfBumps = @event.NumberOfBumps,
            NumberOfLocations = @event.NumberOfLocations
        };
    }
    
    public void Apply(ProductPriceCreated @event)
    {
        _prices.Add(new Price(
            @event.Id,
            @event.DateTimeRange,
            @event.Money));
    }
    
    public void Apply(ProductPriceRemoved @event)
    {
        var price = _prices.First(p => p.Id == @event.Id);
        _prices.Remove(price);
    }
    
    public void Apply(ProductPriceChanged @event)
    {
        var price = _prices.First(p => p.Id == @event.Id);

        _prices.Remove(price);
        _prices.Add(new Price(
            @event.Id,
            @event.DateTimeRange,
            @event.Money));
    }
    
    public void Apply(ProductActivated _)
    {
        IsActive = true;
    }
    
    public void Apply(ProductDeactivated _)
    {
        IsActive = false;
    }
}