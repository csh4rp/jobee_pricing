namespace Jobee.Pricing.Contracts.Commands;

public record CalculatePriceCommand
{
    public required string ProductVersion { get; init; }
}