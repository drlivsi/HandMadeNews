using HandmadeNews.Infrastructure.Options;
using HandmadeNews.Infrastructure.Parsing.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections;

namespace HandmadeNews.UnitTests.Fixture
{
    public class ParsingTestCases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json")
                .Build();

            var services = new ServiceCollection();
            services.Configure<ProducersOptions>(configuration.GetSection("ProducersOptions"));

            var serviceProvider = services.BuildServiceProvider();

            // Retrieve the configured options
            var options = serviceProvider.GetRequiredService<IOptions<ProducersOptions>>().Value;

            yield return new object[] { typeof(LanarteParsingStrategy), options.LanarteUrl, "./Resources/lanarte.txt", 13 };
            yield return new object[] { typeof(BucillaParsingStrategy), options.BucillaUrl, "./Resources/bucilla.txt", 7 };
            yield return new object[] { typeof(KoolerdesignParsingStrategy), options.KoolerDesignUrl, "./Resources/koolerdesign.txt", 10 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
