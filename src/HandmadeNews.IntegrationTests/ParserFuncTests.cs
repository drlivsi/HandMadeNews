using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Xunit;
using HandmadeNews.AzureFunc;
using HandmadeNews.IntegrationTests.Infrastructure;

namespace HandmadeNews.IntegrationTests
{
    [Collection(TestsCollection.Name)]
    public class ParserFuncTests : IClassFixture<IntegrationTestBase>
    {
        readonly ParserFunc _azureFunc;

        public ParserFuncTests(IntegrationTestBase fixture)
        {
            _azureFunc = fixture.ParserFunction;
        }

        [Fact]
        public async Task Parse_Positive()
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());

            // act
            var result = await _azureFunc.Parse(req);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}