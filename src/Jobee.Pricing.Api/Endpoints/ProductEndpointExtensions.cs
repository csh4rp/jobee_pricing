using Jobee.Pricing.Contracts.Commands;
using Jobee.Pricing.Contracts.Models;
using Jobee.Pricing.Contracts.Queries;
using Jobee.Utils.Api.ApiResults;
using Jobee.Utils.Api.Responses;
using Jobee.Utils.Api.Validation;
using Jobee.Utils.Contracts;
using Jobee.Utils.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Jobee.Pricing.Api.Endpoints;

public static class ProductEndpointExtensions
{
    private const string Prefix = "products";

    public static WebApplication AddProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(Prefix);

        group.MapPost(string.Empty, async ([FromBody] CreateProductCommand command,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                var result = await bus.InvokeAsync<CreatedResponse<Guid>>(command, cancellationToken);
                return new CreatedAtResult<Guid>(result);
            })
            .AddEndpointFilter<ValidationEndpointFilter<CreateProductCommand>>()
            .Produces<ProductCreatedResult>(StatusCodes.Status201Created)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("{id:guid}", async ([FromBody] UpdateProductCommand command,
                [FromRoute] Guid id,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                command.SetProductId(id);

                await bus.InvokeAsync(command, cancellationToken);
                return Results.NoContent();
            })
            .AddEndpointFilter<ValidationEndpointFilter<UpdateProductCommand>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<NotFoundErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("{id:guid}/calculate-price", async ([FromBody] CalculatePriceCommand command,
                [FromRoute] Guid id,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                command.SetProductId(id);

                var result = await bus.InvokeAsync<PriceCalculationResult>(command, cancellationToken);
                return Results.Ok(result);
            })
            .AddEndpointFilter<ValidationEndpointFilter<CalculatePriceCommand>>()
            .Produces<PriceCalculationResult>(StatusCodes.Status200OK)
            .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);
        ;

        group.MapDelete("{id:guid}", async ([FromRoute] Guid id,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                var command = new ArchiveProductCommand(id);
                await bus.InvokeAsync(command, cancellationToken);
                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<NotFoundErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("{id:guid}", async ([FromRoute] Guid id,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                var query = new GetProductQuery(id);
                var result = await bus.InvokeAsync<ProductDetailsModel>(query, cancellationToken);
                return Results.Ok(result);
            })
            .Produces<ProductDetailsModel>(StatusCodes.Status200OK)
            .Produces<NotFoundErrorResponse>(StatusCodes.Status404NotFound);
        ;

        group.MapGet(string.Empty, async ([AsParameters] GetProductsQuery query,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                var result = await bus.InvokeAsync<PaginatedResponse<ProductModel>>(query, cancellationToken);
                return PaginatedResult.From(result);
            })
            .Produces<PaginatedResult<ProductModel>>(StatusCodes.Status200OK);

        return app;
    }
}