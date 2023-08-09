using HandmadeNews.Infrastructure.Parsing.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace HandmadeNews.UnitTests.Fixture
{
    public class UnitTestsBase
    {
        public IServiceProvider ServiceProvider { get; }

        public UnitTestsBase()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddScoped<LanarteParsingStrategy>()
                .AddScoped<IParsingStrategy, LanarteParsingStrategy>(s => s.GetRequiredService<LanarteParsingStrategy>());

            serviceCollection
                .AddScoped<BucillaParsingStrategy>()
                .AddScoped<IParsingStrategy, BucillaParsingStrategy>(s => s.GetRequiredService<BucillaParsingStrategy>());

            serviceCollection
                .AddScoped<KoolerdesignParsingStrategy>()
                .AddScoped<IParsingStrategy, KoolerdesignParsingStrategy>(s => s.GetRequiredService<KoolerdesignParsingStrategy>());

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
