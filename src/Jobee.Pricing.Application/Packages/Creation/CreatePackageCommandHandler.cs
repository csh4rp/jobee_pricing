using Jobee.Pricing.Contracts.Packages.Creation;
using Jobee.Pricing.Domain.Common;
using Jobee.Pricing.Domain.Packages;
using Jobee.Pricing.Domain.Products;
using Jobee.Pricing.Domain.Settings;
using Jobee.Utils.Application.Exceptions;
using Jobee.Utils.Contracts;
using Jobee.Utils.Contracts.Responses;
using Microsoft.Extensions.Logging;

namespace Jobee.Pricing.Application.Packages.Creation;

public class CreatePackageCommandHandler
{
    public static async Task<CreatedResponse<Guid>> Handle(CreatePackageCommand command,
        IPackageRepository packageRepository,
        IProductRepository productRepository,
        SettingsService settingsService,
        ILogger<CreatePackageCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        var defaultCurrency = await settingsService.GetDefaultCurrencyAsync(cancellationToken);
        var productExists = await productRepository.ExistsAsync(command.ProductId, cancellationToken);
        
        if (!productExists)
        {
            throw new ValidationException($"Product with id: {command.ProductId} not found", [MemberError.InvalidValue(nameof(command.ProductId), [])]);
        }
        
        var prices = command.Prices.Select(price => 
            new Price(new DateTimeRange(price.StartsAt, price.EndsAt), 
                new Money(price.Amount, defaultCurrency))
        ).ToList();
        
        var package = new Package(command.ProductId, command.Name, command.Description, command.IsActive, command.Quantity, prices);
        
        await packageRepository.AddAsync(package, cancellationToken);
        
        logger.LogInformation("Package with id: {id} and name: {name} created", package.Id, package.Name);
        
        return new CreatedResponse<Guid>
        {
            Id = package.Id
        };
    }
}