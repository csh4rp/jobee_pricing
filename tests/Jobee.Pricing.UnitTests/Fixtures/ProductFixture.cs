using Jobee.Pricing.Domain.Entities;
using Jobee.Pricing.Domain.ValueObjects;

namespace Jobee.Pricing.UnitTests.Fixtures;

public class ProductFixture
{
    public readonly decimal DefaultPrice = 100;
    public readonly decimal CurrentPrice = 105;

    public Product AProduct()
    {
        var product = new Product(
            Guid.NewGuid(),
            "Test Product",
            1,
            true,
            [
                new Price(Guid.CreateVersion7(), new DateTimeRange(), DefaultPrice),
                new Price(Guid.CreateVersion7(), 
                    new DateTimeRange(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 12, 31, 23, 59, 59, TimeSpan.Zero)
                    ), CurrentPrice),
                new Price(Guid.CreateVersion7(), 
                    new DateTimeRange(new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
                        new DateTimeOffset(2028, 12, 31, 23, 59, 59, TimeSpan.Zero)
                    ), 110),
                new Price(Guid.CreateVersion7(), 
                    new DateTimeRange(new DateTimeOffset(2029, 1, 1, 0, 0, 0, TimeSpan.Zero),
                        null
                    ), 120)
            ]);
        
        _  = product.DequeueEvents().ToList();
        
        return product;
    }
}