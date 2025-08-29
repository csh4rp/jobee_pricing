namespace Jobee.Pricing.Domain.Events;

public record ProductCreated(Guid Id, string Name, int NumberOfOffers, bool IsActive, IReadOnlyList<Price> Prices);