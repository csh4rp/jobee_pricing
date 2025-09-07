using FluentValidation;
using FluentValidation.Results;
using Jobee.Pricing.Contracts.Common;
using Jobee.Pricing.Contracts.Creation;
using Jobee.Pricing.Domain.ValueObjects;
using Jobee.Utils.Contracts;

namespace Jobee.Pricing.Application.Common;

public sealed class PricesValidator : AbstractValidator<IReadOnlyList<IPriceModel>>
{
    private const string InfiniteDateRangeError = "Only one price can have an infinite date range";
    private const string OverlappingDateRangeError = "There can't be overlapping date ranges";
    
    public override ValidationResult Validate(ValidationContext<IReadOnlyList<IPriceModel>> context)
    {
        var failures = new List<ValidationFailure>();
        
        foreach (var priceModel in context.InstanceToValidate)
        {
            var range = new DateTimeRange(priceModel.StartsAt, priceModel.EndsAt);
            
            if (range.IsInfinite 
                && context.InstanceToValidate.Any(p => p != priceModel && new DateTimeRange(p.StartsAt, p.EndsAt).IsInfinite)
                && !failures.Any(f => f.ErrorMessage.Equals(InfiniteDateRangeError)))
            {
                var failure = new ValidationFailure(context.PropertyChain.ToString(), InfiniteDateRangeError)
                {
                    ErrorCode = ValidationErrorCodes.MultipleInfinitePricePeriods,
                    AttemptedValue = range.StartsAt
                };
                
                failures.Add(failure);
                context.AddFailure(failure);
            }

            if (!range.IsInfinite 
                && context.InstanceToValidate.Any(p =>
                {
                    var rangeToCheck = new DateTimeRange(p.StartsAt, p.EndsAt);
                    
                    return p != priceModel
                           && (priceModel.StartsAt.HasValue || priceModel.EndsAt.HasValue)
                           && !rangeToCheck.IsInfinite 
                           && rangeToCheck.Overlaps(range);
                })
                && !failures.Any(f => f.ErrorMessage.Equals(OverlappingDateRangeError)))
            {
                var failure = new ValidationFailure(context.PropertyChain.ToString(), OverlappingDateRangeError)
                {
                    ErrorCode = ValidationErrorCodes.OverlappingPricePeriods,
                    AttemptedValue = range.StartsAt
                };
                
                failures.Add(failure);
                context.AddFailure(failure);
            }
        }

        return new ValidationResult(failures);
    }
}