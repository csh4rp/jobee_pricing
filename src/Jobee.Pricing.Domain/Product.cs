using Jobee.Pricing.Domain.Events;

namespace Jobee.Pricing.Domain;

public class Product
{
    private readonly Queue<object> _events = [];
    private readonly List<Price> _prices = [];
    
    public Guid Id { get; private init; }
    
    public bool IsActive { get; private set; }

    public string Name { get; private set; } = null!;
    
    public int NumberOfOffers { get; private set; }
    

    public Product(ProductCreated @event)
    {
        Id = @event.Id;
        IsActive = @event.IsActive;
        Name = @event.Name;
        NumberOfOffers = @event.NumberOfOffers;
        _prices = [.. @event.Prices.Select(p => new Price(p.Id, p.DateRange, p.Amount))];
    }
    
    public Product(Guid id, string name, int numberOfOffers, bool isActive, IReadOnlyList<Price> prices)
    {
        ValidatePrices(prices);
        
        Id = id;
        IsActive = isActive;
        Name = name;
        NumberOfOffers = numberOfOffers;
        _prices = prices as List<Price> ?? prices.ToList();
        _events.Enqueue(new ProductCreated(id, name, numberOfOffers, isActive, prices));
    }

    private static void ValidatePrices(IReadOnlyCollection<Price> prices)
    {
        foreach (var price in prices)
        {
            if (prices.Any(p => p.Id != price.Id && !p.IsDefault && p.DateRange.Overlaps(price.DateRange)))
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
            _events.Enqueue(new ProductChanged(name, numberOfOffers));
        }

        if (isActive != IsActive)
        {
            IsActive = isActive;
        }

        foreach (var price in prices)
        {
            var existingPrice = _prices.FirstOrDefault(p => p.Id == price.Id);
            if (existingPrice is not null && !existingPrice.Equals(price))
            {
                _events.Enqueue(new PriceChanged(existingPrice.Id, existingPrice.DateRange, price.Amount));
            }
            else
            {
                _events.Enqueue(new PriceCreated(price.Id, price.DateRange, price.Amount));
            }
        }

        foreach (var price in _prices)
        {
            var isPriceRemoved = prices.All(p => p.Id != price.Id);
            if (isPriceRemoved)
            {
                _events.Enqueue(new PriceRemoved(price.Id, price.DateRange, price.Amount));
            }
        }
    }

    public decimal GetPrice(DateTimeOffset timestamp)
    {
        var price = _prices.Where(p => 
            p.DateRange.Overlaps(timestamp))
            .OrderBy(t => t.IsDefault ? 0 : 1)
            .First();

        return price.Amount;
    }

    public void Apply(ProductChanged @event)
    {
        Name = @event.Name;
        NumberOfOffers = @event.NumberOfOffers;
    }
    
    public void Apply(PriceCreated @event)
    {
        _prices.Add(new Price(
            @event.Id,
            @event.DateRange,
            @event.Amount));
    }
    
    public void Apply(PriceRemoved @event)
    {
        var price = _prices.FirstOrDefault(p => p.Id == @event.Id);
        if (price is not null)
        {
            _prices.Remove(price);
        }
    }
    
    public void Apply(PriceChanged @event)
    {
        var price = _prices.FirstOrDefault(p => p.Id == @event.Id);
        if (price is null)
        {
            return;
        }

        _prices.Remove(price);
        _prices.Add(new Price(
            @event.Id,
            @event.DateRange,
            @event.Amount));
    }
    
    public IEnumerable<object> DequeueEvents()
    {
        while (_events.Count > 0)
        {
            yield return _events.Dequeue();
        }
    }
}