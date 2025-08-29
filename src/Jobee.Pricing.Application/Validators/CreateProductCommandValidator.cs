using FluentValidation;
using Jobee.Pricing.Contracts.Commands;

namespace Jobee.Pricing.Application.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
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
            .ForEach(f => f.SetValidator(new CreatePriceModelValidator()));
    }
}