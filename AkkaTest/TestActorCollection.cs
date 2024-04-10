using Xunit;

namespace AkkaTest;

[CollectionDefinition(nameof(TestActorFixture))]
public class TestActorCollection : ICollectionFixture<TestActorFixture>
{
}