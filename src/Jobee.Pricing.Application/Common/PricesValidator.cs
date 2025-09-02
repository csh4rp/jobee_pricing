using FluentValidation;
using FluentValidation.Results;
using Jobee.Pricing.Contracts.Common;
using Jobee.Pricing.Contracts.Creation;
using Jobee.Pricing.Domain.ValueObjects;
using Jobee.Utils.Contracts;

namespace Jobee.Pricing.Application.Common;

public sealed class PricesValidator : AbstractValidator<IReadOnlyList<IPriceModel>>
{
    public override ValidationResult Validate(ValidationContext<IReadOnlyList<IPriceModel>> context)
    {
        var failures = new List<ValidationFailure>();
        
        foreach (var priceModel in context.InstanceToValidate)
        {
            var range = new DateTimeRange(priceModel.StartsAt, priceModel.EndsAt);
            
            if (range.IsInfinite && context.InstanceToValidate.Any(i => i != priceModel 
                                                                        && new DateTimeRange(i.StartsAt, i.EndsAt).IsInfinite))
            {
                failures.Add(new ValidationFailure(nameof(priceModel.StartsAt), "Only one price can have an infinite date range")
                {
                    ErrorCode = ErrorCodes.Conflict,
                    AttemptedValue = range.StartsAt
                });
            }

            if (!range.IsInfinite && context.InstanceToValidate.Any(i => i != priceModel
                                                                              && (priceModel.StartsAt.HasValue || priceModel.EndsAt.HasValue)
                                                                              && new DateTimeRange(i.StartsAt, i.EndsAt).Overlaps(range)))
            {
                failures.Add(new ValidationFailure(nameof(priceModel.StartsAt), "There can't be overlapping date ranges")
                {
                    ErrorCode = ErrorCodes.Conflict,
                    AttemptedValue = range.StartsAt
                });
            }
        }
        
        return new ValidationResult(failures);
    }
}