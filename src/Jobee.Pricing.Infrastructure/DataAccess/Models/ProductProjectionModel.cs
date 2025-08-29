namespace Jobee.Pricing.Infrastructure.DataAccess.Models;

public record ProductProjectionModel
{
    public Guid Id { get; init; }

    public string Name { get; set; } = null!;
    
    public int NumberOfOffers { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? LastModifiedAt { get; set; }
    
    public List<PriceProjectionModel> Prices { get; set; } = [];
}