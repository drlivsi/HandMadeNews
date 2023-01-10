using HamdmadeNews.Infrastructure;
using HamdmadeNews.Infrastructure.Data;
using HamdmadeNews.Infrastructure.Options;
using HamdmadeNews.Infrastructure.Parsers.Producers;
using HamdmadeNews.Infrastructure.Scrapers;
using HandmadeNews.AzureFunc.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(HandmadeNews.AzureFunc.Startap))]

namespace HandmadeNews.AzureFunc
{
    public class Startap : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = BuildConfiguration(builder.GetContext().ApplicationRootPath);

            builder.Services.Configure<TelegramOptions>(configuration.GetSection("TelegramOptions"));

            builder.Services.AddAppConfiguration(configuration);

            // builder.Services.AddHttpClient();            
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(connectionString));
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            

            builder.Services
                .AddScoped<LanarteParser>()
                .AddScoped<IParser, LanarteParser>(s => s.GetService<LanarteParser>());

            builder.Services
                .AddScoped<BucillaParser>()
                .AddScoped<IParser, BucillaParser>(s => s.GetService<BucillaParser>());

            builder.Services
                .AddScoped<KoolerdesignParser>()
                .AddScoped<IParser, KoolerdesignParser>(s => s.GetService<KoolerdesignParser>());

            builder.Services.AddScoped<IScrapperService, ScrapperService>();

            //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
        }

        private IConfiguration BuildConfiguration(string applicationRootPath)
        {
            var config =
                new ConfigurationBuilder()
                    .SetBasePath(applicationRootPath)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();


            return config;
        }
    }
}
