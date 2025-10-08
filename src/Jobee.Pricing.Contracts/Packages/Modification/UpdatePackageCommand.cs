using System.Text.Json.Serialization;
using Jobee.Pricing.Contracts.Common;

namespace Jobee.Pricing.Contracts.Packages.Modification;

public record UpdatePackageCommand
{
    [JsonIgnore]
    public Guid PackageId { get; set; }
    
    public required Guid ProductId { get; init; }
    
    public required int Quantity { get; init; }
    
    public required string Name { get; init; }

    public required string Description { get; init; }
    
    public required bool IsActive { get; init; }
    
    public required IReadOnlyList<UpdatePriceModel> Prices { get; init; }
}