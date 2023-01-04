using HandmadeNews.AzureFunc.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeNews.AzureFunc.Extensions
{
    internal static class ConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ProducersOptions>(config.GetSection(nameof(ProducersOptions)));
            return services;
        }
    }
}
