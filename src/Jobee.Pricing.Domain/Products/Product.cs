using Jobee.Pricing.Domain.Events;

namespace Jobee.Pricing.Domain.Products;

public class Product
{
    private readonly Queue<object> _events = [];
    private readonly List<Price> _prices;
    private int _version;

    public Guid Id { get; private init; }
    
    public bool IsActive { get; private set; }

    public string Name { get; private set; }
    
    public int NumberOfOffers { get; private set; }
    
    public IReadOnlyList<Price> Prices => _prices;
    
    public Product(ProductCreated @event)
    {
        Id = @event.Id;
        IsActive = @event.IsActive;
        Name = @event.Name;
        NumberOfOffers = @event.NumberOfOffers;
        _prices = [.. @event.Prices.Select(p => new Price(p.Id, p.DateTimeRange, p.Money))];
    }
    
    public Product(Guid id, string name, int numberOfOffers, bool isActive, IReadOnlyList<Price> prices)
    {
        ValidatePrices(prices);
        
        Id = id;
        IsActive = isActive;
        Name = name;
        NumberOfOffers = numberOfOffers;
        _prices = prices as List<Price> ?? [.. prices];
        _events.Enqueue(new ProductCreated(id, name, numberOfOffers, isActive, prices));
    }

    private static void ValidatePrices(IReadOnlyCollection<Price> prices)
    {
        if (prices.Count(p => p.IsDefault) != 1)
        {
            throw new ArgumentException("Only one default price is allowed.");
        }
        
        foreach (var price in prices)
        {
            if (prices.Any(p => p.Id != price.Id && !p.IsDefault && !price.IsDefault && p.DateTimeRange.Overlaps(price.DateTimeRange)))
            {
                throw new ArgumentException($"Price with ID {price.Id} overlaps with another price in the collection.");
            }
        }
    }

    public void Update(string name, int numberOfOffers, bool isActive, IReadOnlyCollection<Price> prices)
    {
        ValidatePrices(prices);
        
        if (name != Name || numberOfOffers != NumberOfOffers)
        {
            Name = name;
            NumberOfOffers = numberOfOffers;
            EnqueueEvent(new ProductChanged(name, numberOfOffers));
        }

        if (isActive != IsActive)
        {
            IsActive = isActive;
            EnqueueEvent(isActive ? new ProductActivated() : new ProductDeactivated());
        }

        foreach (var price in prices)
        {
            var existingPrice = _prices.FirstOrDefault(p => p.Id == price.Id);
            if (existingPrice is not null && !existingPrice.Equals(price))
            {
                EnqueueEvent(new PriceChanged(existingPrice.Id, existingPrice.DateTimeRange, price.Money));
            }
            else if (existingPrice is null)
            {
                EnqueueEvent(new PriceCreated(price.Id, price.DateTimeRange, price.Money));
            }
        }

        foreach (var price in _prices)
        {
            var isPriceRemoved = prices.All(p => p.Id != price.Id);
            if (isPriceRemoved)
            {
                EnqueueEvent(new PriceRemoved(price.Id, price.DateTimeRange, price.Money));
            }
        }
    }
    
    private void EnqueueEvent(object @event)
    {
        _events.Enqueue(@event);
        _version++;
    }

    public Price GetPrice(DateTimeOffset timestamp)
    {
        var price = _prices.Where(p => 
            p.DateTimeRange.Overlaps(timestamp))
            .OrderBy(t => t.IsDefault ? 1 : 0)
            .First();

        return price;
    }

    public void Apply(ProductChanged @event)
    {
        Name = @event.Name;
        NumberOfOffers = @event.NumberOfOffers;
        _version++;
    }
    
    public void Apply(PriceCreated @event)
    {
        _prices.Add(new Price(
            @event.Id,
            @event.DateTimeRange,
            @event.Money));
        _version++;
    }
    
    public void Apply(PriceRemoved @event)
    {
        var price = _prices.First(p => p.Id == @event.Id);
        _prices.Remove(price);
        _version++;
    }
    
    public void Apply(PriceChanged @event)
    {
        var price = _prices.First(p => p.Id == @event.Id);

        _prices.Remove(price);
        _prices.Add(new Price(
            @event.Id,
            @event.DateTimeRange,
            @event.Money));
        
        _version++;
    }
    
    public void Apply(ProductActivated _)
    {
        IsActive = true;
        _version++;
    }
    
    public void Apply(ProductDeactivated _)
    {
        IsActive = false;
        _version++;
    }
    
    public IEnumerable<object> DequeueEvents()
    {
        while (_events.Count > 0)
        {
            yield return _events.Dequeue();
        }
    }
}