using HamdmadeNews.Infrastructure;
using HamdmadeNews.Infrastructure.Data;
using HamdmadeNews.Infrastructure.Parsers;
using HamdmadeNews.Infrastructure.Scrapers;
using HandmadeNews.AzureFunc.Extensions;
using HandmadeNews.AzureFunc.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Xml.Linq;

[assembly: FunctionsStartup(typeof(HandmadeNews.AzureFunc.Startap))]

namespace HandmadeNews.AzureFunc
{
    public class Startap : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // builder.Services.AddHttpClient();            
            var connectionString = "server=168.119.169.136;uid=kr_hmnews_user;pwd=ypigQoS3TO;database=kr_hmnews";
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(connectionString));
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            

            builder.Services
                .AddScoped<LanarteParser>()
                .AddScoped<IParser, LanarteParser>(s => s.GetService<LanarteParser>());

            builder.Services
                .AddScoped<BucillaParser>()
                .AddScoped<IParser, BucillaParser>(s => s.GetService<BucillaParser>());

            builder.Services.AddScoped<IScrapperService, ScrapperService>();

            //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();

            var configuration = BuildConfiguration(builder.GetContext().ApplicationRootPath);
            builder.Services.AddAppConfiguration(configuration);
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
