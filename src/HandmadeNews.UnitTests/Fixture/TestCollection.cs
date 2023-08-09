using Xunit;

namespace HandmadeNews.UnitTests.Fixture
{
    [CollectionDefinition(Name)]
    public class TestsCollection : ICollectionFixture<UnitTestsBase>
    {
        public const string Name = nameof(TestsCollection);
    }
}
