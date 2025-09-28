using AwesomeAssertions;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Common.ValueObjects;

namespace Jobee.Pricing.UnitTests.Domain;

public class DateTimeRangeTests
{
    [Theory]
    [MemberData(nameof(ActiveDateRanges))]
    public void ShouldOverlapRange_WhenDateRangeIsInfinite(DateTimeRange dateTimeRange)
    {
        var range = new DateTimeRange(null, null);
        
        var overlaps = range.Overlaps(dateTimeRange);
        
        overlaps.Should().BeTrue();
    }
    
    [Theory]
    [MemberData(nameof(ActiveDateRanges))]
    public void ShouldOverlapRange_WhenDateRangeIsActive(DateTimeRange dateTimeRange)
    {
        var range = new DateTimeRange(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        
        var overlaps = range.Overlaps(dateTimeRange);
        
        overlaps.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ActiveDateRanges))]
    public void ShouldOverlapRange_WhenDateRangeHasOnlyStartDate(DateTimeRange dateTimeRange)
    {
        var range = new DateTimeRange(DateTimeOffset.UtcNow.AddDays(-1), null);
        
        var overlaps = range.Overlaps(dateTimeRange);
        
        overlaps.Should().BeTrue();
    }
    
    [Theory]
    [MemberData(nameof(ActiveDateRanges))]
    public void ShouldOverlapRange_WhenDateRangeHasOnlyEndDate(DateTimeRange dateTimeRange)
    {
        var range = new DateTimeRange(null, DateTimeOffset.UtcNow.AddDays(1));
        
        var overlaps = range.Overlaps(dateTimeRange);
        
        overlaps.Should().BeTrue();
    }
    
    [Theory]
    [MemberData(nameof(NotMatchingRanges))]
    public void ShouldNotOverlapRange_WhenDateRangeIsOutside(DateTimeRange range, DateTimeRange otherRange)
    {
        var overlaps = range.Overlaps(otherRange);
        
        overlaps.Should().BeFalse();
    }
    
    [Fact]
    public void ShouldOverlapTimestamp_WhenRangeIsInfinite()
    {
        var range = new DateTimeRange();
        var timestamp = DateTimeOffset.UtcNow;
        
        var overlaps = range.Overlaps(timestamp);
        
        overlaps.Should().BeTrue();
    }

    [Fact]
    public void ShouldOverlapTimestamp_WhenDateRangeHasOnlyStartDate()
    {
        var range = new DateTimeRange(DateTimeOffset.UtcNow.AddDays(-1), null);
        var timestamp = DateTimeOffset.UtcNow;
        
        var overlaps = range.Overlaps(timestamp);
        
        overlaps.Should().BeTrue();
    }
    
    [Fact]
    public void ShouldOverlapTimestamp_WhenDateRangeHasOnlyEndDate()
    {
        var range = new DateTimeRange(null, DateTimeOffset.UtcNow.AddDays(2));
        var timestamp = DateTimeOffset.UtcNow;
        
        var overlaps = range.Overlaps(timestamp);
        
        overlaps.Should().BeTrue();
    }
    
    [Fact]
    public void ShouldOverlapTimestamp_WhenDateRangeHasBothDates()
    {
        var range = new DateTimeRange(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(2));
        var timestamp = DateTimeOffset.UtcNow;
        
        var overlaps = range.Overlaps(timestamp);
        
        overlaps.Should().BeTrue();
    }
    
    public static IEnumerable<object[]> ActiveDateRanges()
    {
        yield return [new DateTimeRange(null, null)];
        yield return [new DateTimeRange(DateTimeOffset.MinValue, null)];
        yield return [new DateTimeRange(null, DateTimeOffset.MaxValue)];
        yield return [new DateTimeRange(DateTimeOffset.MinValue, DateTimeOffset.MaxValue)];
    }
    
    public static IEnumerable<object[]> NotMatchingRanges()
    {
        yield return [new DateTimeRange(DateTimeOffset.UtcNow.AddDays(-1), null), new DateTimeRange(DateTimeOffset.UtcNow.AddDays(-4), DateTimeOffset.UtcNow.AddDays(-2))];
        yield return [new DateTimeRange(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1)), new DateTimeRange(DateTimeOffset.UtcNow.AddDays(2), null)];
    }
}