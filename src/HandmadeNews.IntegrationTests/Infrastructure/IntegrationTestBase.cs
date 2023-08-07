using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HandmadeNews.AzureFunc;
using HandmadeNews.Infrastructure.Services;

namespace HandmadeNews.IntegrationTests.Infrastructure
{
    public class IntegrationTestBase : MySqlTestBase
    {
        public ParserFunc ParserFunction { get; set; }

        public IntegrationTestBase(TestHost testHost)
        {
            ParserFunction = new ParserFunc(testHost.ServiceProvider.GetRequiredService<ILogger<ParserFunc>>(), testHost.ServiceProvider.GetRequiredService<IParsingService>());
        }

        protected override string DockerComposeFileFullPath()
        {
            string composeFile = Path.Combine(Directory.GetCurrentDirectory(), @"Infrastructure\docker-compose.yml");
            if (!File.Exists(composeFile))
            {
                throw new FileNotFoundException($"docker-compose file {composeFile} not found.");
            }
            return composeFile;
        }

        protected override string TestSettingsFileFullPath()
        {
            string settingsFile = Path.Combine(Directory.GetCurrentDirectory(), @"Infrastructure\test-settings.json");
            if (!File.Exists(settingsFile))
            {
                throw new FileNotFoundException($"Settings file {settingsFile} not found.");
            }
            return settingsFile;
        }
    }
}
