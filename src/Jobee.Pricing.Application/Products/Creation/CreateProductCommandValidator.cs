using FluentValidation;
using Jobee.Pricing.Application.Common;
using Jobee.Pricing.Application.Products.Common;
using Jobee.Pricing.Contracts.Products.Creation;

namespace Jobee.Pricing.Application.Products.Creation;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Attributes)
            .NotNull()
            .SetValidator(new AttributesValidator());
            
        RuleFor(x => x.Prices)
            .NotEmpty()
            .SetValidator(new PricesValidator())
            .ForEach(f => f.SetValidator(new PriceModelValidator()));
    }
}