using FluentValidation;
using Jobee.Pricing.Contracts.Products.Common;

namespace Jobee.Pricing.Application.Products.Common;

public class AttributesValidator : AbstractValidator<AttributesModel>
{
    public AttributesValidator()
    {
        RuleFor(x => x.NumberOfBumps)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(x => x.NumberOfLocations)
            .GreaterThanOrEqualTo(1);
        
        RuleFor(x => x.DurationInDays)
            .GreaterThanOrEqualTo(1);
    }
}