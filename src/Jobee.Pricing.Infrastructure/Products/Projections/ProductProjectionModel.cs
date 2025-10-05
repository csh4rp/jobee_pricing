using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Infrastructure.Products.Projections;

public record ProductProjectionModel
{
    public Guid Id { get; init; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? LastModifiedAt { get; set; }
    
    public List<Price> Prices { get; set; } = [];
}