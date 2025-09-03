using FluentValidation;
using Jobee.Pricing.Application.Common;
using Jobee.Pricing.Contracts.Modification;

namespace Jobee.Pricing.Application.Modification;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.NumberOfOffers)
            .GreaterThan(0);

        RuleFor(x => x.Prices)
            .NotEmpty()
            .SetValidator(new PricesValidator())
            .ForEach(f => f.SetValidator(new PriceModelValidator()));
    }
}