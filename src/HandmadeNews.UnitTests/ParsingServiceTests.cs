using FluentAssertions;
using HandmadeNews.Domain.SeedWork;
using HandmadeNews.Infrastructure.Options;
using HandmadeNews.Infrastructure.Parsing;
using HandmadeNews.Infrastructure.Services;
using HandmadeNews.UnitTests.Fixture;
using HandmadeNews.UnitTests.HttpHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;
using Xunit;

namespace HandmadeNews.UnitTests
{
    [Collection(TestsCollection.Name)]
    public class ParsingServiceTests: IClassFixture<UnitTestsBase>
    {
        private readonly ILogger<ParsingService> _logger = Substitute.For<ILogger<ParsingService>>();
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IParser _parser = new Parser();

        public ParsingServiceTests(UnitTestsBase unitTestsBase)
        {
            _serviceProvider = unitTestsBase.ServiceProvider;
        }

        [Theory]
        [ClassData(typeof(ParsingTestCases))]
        public async Task DownloadAndParse_Success(Type strategy, string url, string resourcesPath, int expectedCount)
        {
            // Arrange
            var telegramOptions = Options.Create(new TelegramOptions());
            var producers = new Dictionary<Type, string> { { strategy, url } };
            var html = await File.ReadAllTextAsync(resourcesPath);
            var httpClient = Substitute.For<HttpClient>(new MockHttpMessageHandler(html, HttpStatusCode.OK));
            var sut = new ParsingService(_logger, telegramOptions, _serviceProvider, _unitOfWork, _parser, httpClient);

            // Act
            var downloadedHtmlList = await sut.DownloadAsync(producers);
            var parsedArticles = sut.Parse(downloadedHtmlList);

            // Assert
            downloadedHtmlList.Should().NotBeNull().And.HaveCount(1);
            parsedArticles.Should().NotBeNull().And.HaveCount(expectedCount);
        }
    }
}