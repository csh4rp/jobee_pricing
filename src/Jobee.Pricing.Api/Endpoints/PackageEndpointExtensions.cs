using Jobee.Pricing.Contracts.Packages.Archiving;
using Jobee.Pricing.Contracts.Packages.Calculation;
using Jobee.Pricing.Contracts.Packages.Creation;
using Jobee.Pricing.Contracts.Packages.Modification;
using Jobee.Pricing.Contracts.Products.Archiving;
using Jobee.Pricing.Contracts.Products.Calculation;
using Jobee.Pricing.Contracts.Products.Creation;
using Jobee.Pricing.Contracts.Products.Models;
using Jobee.Pricing.Contracts.Products.Modification;
using Jobee.Pricing.Contracts.Products.Queries;
using Jobee.Utils.Api.ApiResults;
using Jobee.Utils.Api.Responses;
using Jobee.Utils.Api.Validation;
using Jobee.Utils.Contracts;
using Jobee.Utils.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Jobee.Pricing.Api.Endpoints;

public static class PackageEndpointExtensions
{
    private const string Prefix = "packages";

    public static WebApplication AddPackageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(Prefix);

        group.MapPost(string.Empty, async ([FromBody] CreatePackageCommand command,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                var result = await bus.InvokeAsync<CreatedResponse<Guid>>(command, cancellationToken);
                return new CreatedAtResult<Guid>(result);
            })
            .AddEndpointFilter<ValidationEndpointFilter<CreatePackageCommand>>()
            .Produces<CreatedResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("{id:guid}", async ([FromBody] UpdatePackageCommand command,
                [FromRoute] Guid id,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                command.PackageId = id;

                await bus.InvokeAsync(command, cancellationToken);
                return Results.NoContent();
            })
            .AddEndpointFilter<ValidationEndpointFilter<UpdatePackageCommand>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<NotFoundErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("calculate-price", async ([FromBody] CalculatePackagePriceCommand command,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                var result = await bus.InvokeAsync<PackagePriceCalculationResult>(command, cancellationToken);
                return Results.Ok(result);
            })
            .AddEndpointFilter<ValidationEndpointFilter<CalculatePackagePriceCommand>>()
            .Produces<ProductPriceCalculationResult>(StatusCodes.Status200OK)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("{id:guid}", async ([FromRoute] Guid id,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                var command = new ArchivePackageCommand(id);
                await bus.InvokeAsync(command, cancellationToken);
                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<NotFoundErrorResponse>(StatusCodes.Status404NotFound);

        return app;
    }
}