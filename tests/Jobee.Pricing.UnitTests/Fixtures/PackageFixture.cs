using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;
using Jobee.Pricing.Domain.Packages;

namespace Jobee.Pricing.UnitTests.Fixtures;

public class PackageFixture
{
    public readonly Money DefaultPrice = new(100, Currency.PLN);
    public readonly Money CurrentPrice = new(105, Currency.PLN);
    
    public Package APackage()
    {
        var package = new Package(Guid.CreateVersion7(), "Package-1", "Package Description", true,
            1, [
                new Price(Guid.CreateVersion7(), new DateTimeRange(), DefaultPrice),
                new Price(Guid.CreateVersion7(),
                    new DateTimeRange(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                        new DateTimeOffset(2026, 12, 31, 23, 59, 59, TimeSpan.Zero)
                    ), CurrentPrice),
                new Price(Guid.CreateVersion7(),
                    new DateTimeRange(new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero),
                        new DateTimeOffset(2028, 12, 31, 23, 59, 59, TimeSpan.Zero)
                    ), new Money(110, Currency.EUR)),
                new Price(Guid.CreateVersion7(),
                    new DateTimeRange(new DateTimeOffset(2029, 1, 1, 0, 0, 0, TimeSpan.Zero),
                        null
                    ), new Money(120, Currency.EUR))
            ]);

        return package;
    }
}