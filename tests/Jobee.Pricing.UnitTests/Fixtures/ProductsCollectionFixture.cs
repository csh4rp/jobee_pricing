namespace Jobee.Pricing.UnitTests.Fixtures;

[CollectionDefinition("Products")]
public class ProductsCollectionFixture : ICollectionFixture<ProductFixture>, ICollectionFixture<TestTimeProvider>;