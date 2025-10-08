using FluentValidation;
using Jobee.Pricing.Application.Common;
using Jobee.Pricing.Contracts.Packages.Creation;

namespace Jobee.Pricing.Application.Packages.Creation;

public class CreatePackageCommandValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Quantity)
            .GreaterThan(1);

        RuleFor(x => x.ProductId)
            .NotEmpty();
        
        RuleFor(x => x.Prices)
            .NotEmpty()
            .SetValidator(new PricesValidator())
            .ForEach(f => f.SetValidator(new PriceModelValidator()));
    }
}