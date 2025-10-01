namespace Jobee.Pricing.Contracts.Products.Queries;

public record GetCurrentProductPricesQuery
{
    public int PageNumber { get; init; } = 1;
    
    public int PageSize { get; init; } = 10;
    
    public bool? IsActive { get; init; }
}