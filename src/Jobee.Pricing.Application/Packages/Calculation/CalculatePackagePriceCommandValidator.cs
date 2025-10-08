using FluentValidation;
using Jobee.Pricing.Contracts.Packages.Calculation;

namespace Jobee.Pricing.Application.Packages.Calculation;

public class CalculatePackagePriceCommandValidator : AbstractValidator<CalculatePackagePriceCommand>
{
    public CalculatePackagePriceCommandValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty();

        RuleFor(x => x.Currency)
            .NotEmpty();
    }
}