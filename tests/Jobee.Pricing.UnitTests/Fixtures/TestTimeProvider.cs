namespace Jobee.Pricing.UnitTests.Fixtures;


public class TestTimeProvider : TimeProvider
{
    public DateTimeOffset? CurrentDate { get; set; }

    public override DateTimeOffset GetUtcNow()
    {
        return CurrentDate ?? base.GetUtcNow();
    }
}