using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Domain.Packages;

public class Package : Entity<Guid>
{
    private readonly List<Price> _prices;
    
    public Guid ProductId { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public int Quantity { get; private set; }
    
    public IReadOnlyList<Price> Prices => _prices;
    
    public Package(Guid productId, string name, string description, bool isActive, int quantity,
        IReadOnlyList<Price> prices)
    {
        PriceValidator.ValidatePrices(prices);
        
        Id = Guid.CreateVersion7();
        ProductId = productId;
        Name = name;
        Description = description;
        IsActive = isActive;
        Quantity = quantity;
        _prices = prices as List<Price> ?? [.. prices];
    }
    
    public void Update(string name, string description, bool isActive, int quantity,
        IReadOnlyList<Price> prices)
    {
        PriceValidator.ValidatePrices(prices);
        
        if (name != Name)
        {
            Name = name;
            EnqueueEvent(new PackageNameChanged(name));
        }

        if (description != Description)
        {
            Description = description;
            EnqueueEvent(new PackageDescriptionChanged(description));
        }

        if (isActive != IsActive)
        {
            IsActive = isActive;
            EnqueueEvent(isActive ? new PackageActivated() : new PackageDeactivated());
        }
        
        if (quantity != Quantity)
        {
            Quantity = quantity;
            EnqueueEvent(new PackageQuantityChanged(quantity));
        }

        foreach (var price in prices)
        {
            var existingPrice = _prices.FirstOrDefault(p => p.Id == price.Id);
            if (existingPrice is not null && !existingPrice.Equals(price))
            {
                _prices.Remove(existingPrice);
                _prices.Add(price);
                EnqueueEvent(new PackagePriceChanged(existingPrice.Id, existingPrice.DateTimeRange, price.Value));
            }
            else if (existingPrice is null)
            {
                _prices.Add(price);
                EnqueueEvent(new PackagePriceCreated(price.Id, price.DateTimeRange, price.Value));
            }
        }

        var pricesToRemove = new List<Guid>();
        foreach (var price in _prices)
        {
            var isPriceRemoved = prices.All(p => p.Id != price.Id);
            if (isPriceRemoved)
            {
                pricesToRemove.Add(price.Id);
                EnqueueEvent(new PackagePriceRemoved(price.Id, price.DateTimeRange, price.Value));
            }
        }

        _prices.RemoveAll(p => pricesToRemove.Contains(p.Id));
    }
    
    public void Apply(PackageCreated @event)
    {
        Id = @event.Id;
        ProductId = @event.ProductId;
        Name = @event.Name;
        Description = @event.Description;
        IsActive = @event.IsActive;
        Quantity = @event.Quantity;
        _prices.Clear();
        _prices.AddRange(@event.Prices);
    }
    
    public void Apply(PackageNameChanged @event)
    {
        Name = @event.Name;
    }
    
    public void Apply(PackageDescriptionChanged @event)
    {
        Description = @event.Description;
    }
    
    public void Apply(PackageQuantityChanged @event)
    {
        Quantity = @event.Quantity;
    }
    
    public void Apply(PackageActivated _)
    {
        IsActive = true;
    }
    
    public void Apply(PackageDeactivated _)
    {
        IsActive = false;
    }
    
    public void Apply(PackagePriceCreated @event)
    {
        _prices.Add(new Price(
            @event.Id,
            @event.DateTimeRange,
            @event.Value));
    }
    
    public void Apply(PackagePriceRemoved @event)
    {
        var price = _prices.First(p => p.Id == @event.Id);
        _prices.Remove(price);
    }
    
    public void Apply(PackagePriceChanged @event)
    {
        var price = _prices.First(p => p.Id == @event.Id);

        _prices.Remove(price);
        _prices.Add(new Price(
            @event.Id,
            @event.DateTimeRange,
            @event.Value));
    }

    public Price GetPrice(DateTimeOffset timestamp)
    {
        var price = _prices.Where(p => 
                p.DateTimeRange.Overlaps(timestamp))
            .OrderBy(t => t.IsDefault ? 1 : 0)
            .First();

        return price;
    }
}