using Xunit;

namespace HandmadeNews.IntegrationTests.Fixture
{
    [CollectionDefinition(Name)]
    public class TestsCollection : ICollectionFixture<TestHost>
    {
        public const string Name = nameof(TestsCollection);
    }
}