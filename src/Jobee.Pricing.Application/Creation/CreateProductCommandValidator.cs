using FluentValidation;
using Jobee.Pricing.Application.Common;
using Jobee.Pricing.Contracts.Creation;

namespace Jobee.Pricing.Application.Creation;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
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