using FluentValidation;
using Jobee.Pricing.Contracts.Models;

namespace Jobee.Pricing.Application.Validators;

public class CreatePriceModelValidator : AbstractValidator<CreatePriceModel>
{
    public CreatePriceModelValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0);
    }
}