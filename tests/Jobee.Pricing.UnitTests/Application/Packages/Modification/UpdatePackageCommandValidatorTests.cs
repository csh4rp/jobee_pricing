using AwesomeAssertions;
using Jobee.Pricing.Application.Common;
using Jobee.Pricing.Application.Packages.Modification;
using Jobee.Pricing.Contracts.Packages.Modification;

namespace Jobee.Pricing.UnitTests.Application.Packages.Modification;

public class UpdatePackageCommandValidatorTests
{
    private readonly UpdatePackageCommandValidator _validator = new();

    [Fact]
    public void ShouldReturnNoErrors_WhenCommandIsValid()
    {
        var command = new UpdatePackageCommand
        {
            PackageId = Guid.NewGuid(),
            Name = "Test Package",
            Description = "Description",
            ProductId = Guid.NewGuid(),
            IsActive = true,
            Quantity = 2,
            Prices = new List<Contracts.Common.UpdatePriceModel>
            {
                new()
                {
                    Id = null,
                    Amount = 10,
                    EndsAt = null,
                    StartsAt = null
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Amount = 20,
                    StartsAt = DateTimeOffset.UtcNow.AddDays(1),
                    EndsAt = null
                }
            }
        };
        
        var validationResult = _validator.Validate(command);
        
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void ShouldReturnErrors_WhenCommandIsInvalid()
    {
        var command = new UpdatePackageCommand
        {
            PackageId = Guid.Empty,
            Name = "",
            Description = "",
            ProductId = Guid.Empty,
            IsActive = true,
            Quantity = 0,
            Prices = new List<Contracts.Common.UpdatePriceModel>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Amount = -10,
                    StartsAt = null,
                    EndsAt = null
                },
                new()
                {
                    Id = null,
                    Amount = 20,
                    StartsAt = null,
                    EndsAt = null
                },
                new()
                {
                    Id = null,
                    Amount = 20,
                    StartsAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    EndsAt = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero)
                },
                new()
                {
                    Id = null,
                    Amount = 20,
                    StartsAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    EndsAt = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero)
                }
            }
        };
        
        var validationResult = _validator.Validate(command);
        
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(7);
        
        validationResult.Errors.Should().Contain(e => e.ErrorCode == "NotEmptyValidator"
                                                      && e.PropertyName == nameof(UpdatePackageCommand.Name));
        
        validationResult.Errors.Should().Contain(e => e.ErrorCode == "NotEmptyValidator"
                                                      && e.PropertyName == nameof(UpdatePackageCommand.Description));
        
        validationResult.Errors.Should().Contain(e => e.ErrorCode == "NotEmptyValidator"
                                                      && e.PropertyName == nameof(UpdatePackageCommand.ProductId));

        validationResult.Errors.Should().Contain(e => e.ErrorCode == "GreaterThanValidator"
                                                      && e.PropertyName == nameof(UpdatePackageCommand.Quantity));

        validationResult.Errors.Should().Contain(e => e.ErrorCode == ValidationErrorCodes.MultipleInfinitePricePeriods
                                                      && e.PropertyName == nameof(UpdatePackageCommand.Prices));
        
        validationResult.Errors.Should().Contain(e => e.ErrorCode == ValidationErrorCodes.OverlappingPricePeriods
                                                      && e.PropertyName == nameof(UpdatePackageCommand.Prices));
    }
}