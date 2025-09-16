namespace Jobee.Pricing.Contracts.Products.Queries;

public record GetProductsQuery
{
    public required string? Name { get; init; } 

    public required bool? IsActive { get; init; }
    
    public required int PageNumber { get; init; } = 1;
    
    public required int PageSize { get; init; } = 10;
}