using AwesomeAssertions;
using Jobee.Pricing.Application.Common;
using Jobee.Pricing.Application.Products.Common;
using Jobee.Pricing.Application.Products.Creation;
using Jobee.Pricing.Contracts.Products.Common;
using Jobee.Pricing.Contracts.Products.Creation;

namespace Jobee.Pricing.UnitTests.Application.Products.Creation;

public class CreateProductValidatorTests
{
    private static readonly CreateProductCommandValidator Validator = new();

    [Fact]
    public void ShouldReturnNoErrors_WhenCommandIsValid()
    {
        var command = new CreateProductCommand
        {
            IsActive = true,
            Name = "Valid Product",
            Description = "A valid product description",
            Attributes = new AttributesModel
            {
                NumberOfBumps = 1,
                NumberOfLocations = 1,
                DurationInDays = 30
            },
            FeatureFlags = new FeatureFlagsModel
            {
                HasPriority = false
            },
            Prices = new List<CreatePriceModel>
            {
                new()
                {
                    Amount = 10,
                    StartsAt = null,
                    EndsAt = null
                },
                new()
                {
                    Amount = 20,
                    StartsAt = DateTimeOffset.UtcNow,
                    EndsAt = null
                }
            }
        };

        var validationResult = Validator.Validate(command);
        
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldReturnErrors_WhenCommandIsInvalid()
    {
        var command = new CreateProductCommand
        {
            IsActive = true,
            Name = "",
            Description = "",
            Attributes = new AttributesModel
            {
                NumberOfBumps = -1,
                NumberOfLocations = 0,
                DurationInDays = 0
            },
            FeatureFlags = new FeatureFlagsModel
            {
                HasPriority = false
            },
            Prices = new List<CreatePriceModel>
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

        var validationResult = Validator.Validate(command);
        
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(5);
        validationResult.Errors.Should().Contain(e => e.ErrorCode == "NotEmptyValidator"
                                                      && e.PropertyName == nameof(CreateProductCommand.Name));

        validationResult.Errors.Should().Contain(e => e.ErrorCode == "GreaterThanOrEqualValidator"
                                                      && e.PropertyName == "Prices[0].Amount");

        validationResult.Errors.Should().Contain(e => e.ErrorCode == ValidationErrorCodes.MultipleInfinitePricePeriods
                                                      && e.PropertyName == nameof(CreateProductCommand.Prices));
        
        validationResult.Errors.Should().Contain(e => e.ErrorCode == ValidationErrorCodes.OverlappingPricePeriods
                                                      && e.PropertyName == nameof(CreateProductCommand.Prices));
    }

}