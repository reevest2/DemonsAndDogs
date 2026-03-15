namespace DemonsAndDogs.E2E.Tests.Fixtures;

[CollectionDefinition("E2E")]
public class E2ECollection : ICollectionFixture<ServerFixture>, ICollectionFixture<PlaywrightFixture>
{
}
