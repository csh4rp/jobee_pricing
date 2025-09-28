using FluentValidation;
using Jobee.Pricing.Application.Common;
using Jobee.Pricing.Application.Products.Common;
using Jobee.Pricing.Contracts.Products.Modification;

namespace Jobee.Pricing.Application.Products.Modification;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Attributes)
            .SetValidator(new AttributesValidator());

        RuleFor(x => x.Prices)
            .NotEmpty()
            .SetValidator(new PricesValidator())
            .ForEach(f => f.SetValidator(new PriceModelValidator()));
    }
}