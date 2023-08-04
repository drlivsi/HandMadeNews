using HandmadeNews.AzureFunc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(LoggingStartup))]
namespace HandmadeNews.AzureFunc
{
    public class LoggingStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Replace ILogger<T> with the one that works fine in all scenarios 
            var logger = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(ILogger<>));
            if (logger != null)
                builder.Services.Remove(logger);

            builder.Services.Add(new ServiceDescriptor(typeof(ILogger<>), typeof(FunctionsLogger<>), ServiceLifetime.Transient));
        }

        class FunctionsLogger<T> : ILogger<T>
        {
            readonly ILogger logger;
            public FunctionsLogger(ILoggerFactory factory)
                // See https://github.com/Azure/azure-functions-host/issues/4689#issuecomment-533195224
                => logger = factory.CreateLogger(LogCategories.CreateFunctionUserCategory(typeof(T).FullName));
            public IDisposable BeginScope<TState>(TState state) => logger.BeginScope(state);
            public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
                => logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
