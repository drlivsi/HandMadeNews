using HandmadeNews.Infrastructure.Data;
using HandmadeNews.Infrastructure.Options;
using HandmadeNews.Infrastructure.Parsing;
using HandmadeNews.Infrastructure.Parsing.Strategies;
using HandmadeNews.Infrastructure.Repositories;
using HandmadeNews.Infrastructure.Services;
using HandmadeNews.AzureFunc.Extensions;
using HandmadeNews.Domain.SeedWork;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(HandmadeNews.AzureFunc.Startup))]

namespace HandmadeNews.AzureFunc
{
    public class Startup : FunctionsStartup
    {
        public IConfiguration Configuration { get; private set; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            InitializeConfiguration(builder);

            //var configuration = BuildConfiguration(builder.GetContext().ApplicationRootPath);

            builder.Services.Configure<TelegramOptions>(Configuration.GetSection("TelegramOptions"));

            builder.Services.AddAppConfiguration(Configuration);
            
            builder.Services.AddHttpClient();            
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(connectionString));
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IParser, Parser>();

            builder.Services
                .AddScoped<LanarteParsingStrategy>()
                .AddScoped<IParsingStrategy, LanarteParsingStrategy>(s => s.GetService<LanarteParsingStrategy>());

            builder.Services
                .AddScoped<BucillaParsingStrategy>()
                .AddScoped<IParsingStrategy, BucillaParsingStrategy>(s => s.GetService<BucillaParsingStrategy>());

            builder.Services
                .AddScoped<KoolerdesignParsingStrategy>()
                .AddScoped<IParsingStrategy, KoolerdesignParsingStrategy>(s => s.GetService<KoolerdesignParsingStrategy>());

            builder.Services.AddScoped<IParsingService, ParsingService>();
        }

        private void InitializeConfiguration(IFunctionsHostBuilder builder)
        {
            var executionContextOptions = builder
                .Services
                .BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>()
                .Value;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(executionContextOptions.AppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        //private IConfiguration BuildConfiguration(string applicationRootPath)
        //{
        //    var config =
        //        new ConfigurationBuilder()
        //            .SetBasePath(applicationRootPath)
        //            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        //            .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
        //            .AddEnvironmentVariables()
        //            .Build();


        //    return config;
        //}
    }
}
