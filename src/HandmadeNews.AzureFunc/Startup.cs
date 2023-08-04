using Google.Protobuf.WellKnownTypes;
using HandmadeNews.Infrastructure;
using HandmadeNews.Infrastructure.Data;
using HandmadeNews.Infrastructure.Options;
using HandmadeNews.Infrastructure.Parsing;
using HandmadeNews.Infrastructure.Parsing.Strategies;
using HandmadeNews.Infrastructure.Repositories;
using HandmadeNews.Infrastructure.Services;
using HandmadeNews.AzureFunc.Extensions;
using HandmadeNews.Domain.SeedWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using HandmadeNews.Infrastructure.Data;
using static System.Formats.Asn1.AsnWriter;

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
            
            builder.Services.AddHttpClient();            
            var connectionString = configuration.GetConnectionString("DefaultConnection");

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

            builder.Services.AddScoped<IScrapperService, ScrapperService>();
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
