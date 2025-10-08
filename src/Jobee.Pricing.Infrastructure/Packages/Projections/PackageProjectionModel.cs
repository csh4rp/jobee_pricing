using Jobee.Pricing.Domain.Common;

namespace Jobee.Pricing.Infrastructure.Packages.Projections;

public record PackageProjectionModel
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }
    
    public int Quantity { get; set; }
    
    public List<Price> Prices { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? LastModifiedAt { get; set; }
}