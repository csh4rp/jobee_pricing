namespace Jobee.Pricing.Domain.Products;

public record ProductCreated(Guid Id, string Name, int NumberOfOffers, bool IsActive, IReadOnlyList<Price> Prices);