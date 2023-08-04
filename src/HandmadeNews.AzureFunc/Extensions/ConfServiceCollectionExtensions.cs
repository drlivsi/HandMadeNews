using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HandmadeNews.Infrastructure.Options;

namespace HandmadeNews.AzureFunc.Extensions
{
    internal static class ConfServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ProducersOptions>(config.GetSection(nameof(ProducersOptions)));
            return services;
        }
    }
}
