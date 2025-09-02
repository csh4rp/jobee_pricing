using FluentValidation;
using Jobee.Pricing.Contracts.Common;

namespace Jobee.Pricing.Application.Common;

public sealed class PriceModelValidator : AbstractValidator<IPriceModel>
{
    public PriceModelValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0);

        When(x => x.StartsAt.HasValue && x.EndsAt.HasValue, () =>
        {
            RuleFor(c => c.StartsAt).LessThan(c => c.EndsAt);
        });
    }
}