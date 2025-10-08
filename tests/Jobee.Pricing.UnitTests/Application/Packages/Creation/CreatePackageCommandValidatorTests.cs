using AwesomeAssertions;
using Jobee.Pricing.Application.Common;
using Jobee.Pricing.Application.Packages.Creation;
using Jobee.Pricing.Contracts.Packages.Creation;

namespace Jobee.Pricing.UnitTests.Application.Packages.Creation;

public class CreatePackageCommandValidatorTests
{
    private readonly CreatePackageCommandValidator _validator = new();

    [Fact]
    public void ShouldReturnNoErrors_WhenCommandIsValid()
    {
        var command = new CreatePackageCommand
        {
            Name = "Test Package",
            Description = "Description",
            ProductId = Guid.NewGuid(),
            IsActive = true,
            Quantity = 2,
            Prices = new List<Contracts.Common.CreatePriceModel>
            {
                new()
                {
                    Amount = 10,
                    EndsAt = null,
                    StartsAt = null
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
        var command = new CreatePackageCommand
        {
            Name = "",
            Description = "",
            ProductId = Guid.Empty,
            IsActive = true,
            Quantity = 0,
            Prices = new List<Contracts.Common.CreatePriceModel>
            {
                new()
                {
                    Amount = -10,
                    StartsAt = null,
                    EndsAt = null
                },
                new()
                {
                    Amount = 20,
                    StartsAt = null,
                    EndsAt = null
                },
                new()
                {
                    Amount = 20,
                    StartsAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    EndsAt = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero)
                },
                new()
                {
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
                                                      && e.PropertyName == nameof(CreatePackageCommand.Name));
        
        validationResult.Errors.Should().Contain(e => e.ErrorCode == "NotEmptyValidator"
                                                      && e.PropertyName == nameof(CreatePackageCommand.Description));
        
        validationResult.Errors.Should().Contain(e => e.ErrorCode == "NotEmptyValidator"
                                                      && e.PropertyName == nameof(CreatePackageCommand.ProductId));

        validationResult.Errors.Should().Contain(e => e.ErrorCode == "GreaterThanValidator"
                                                      && e.PropertyName == nameof(CreatePackageCommand.Quantity));

        validationResult.Errors.Should().Contain(e => e.ErrorCode == ValidationErrorCodes.MultipleInfinitePricePeriods
                                                      && e.PropertyName == nameof(CreatePackageCommand.Prices));
        
        validationResult.Errors.Should().Contain(e => e.ErrorCode == ValidationErrorCodes.OverlappingPricePeriods
                                                      && e.PropertyName == nameof(CreatePackageCommand.Prices));
    }
}