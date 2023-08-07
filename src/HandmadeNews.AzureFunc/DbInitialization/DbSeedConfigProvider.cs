using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.EntityFrameworkCore;
using HandmadeNews.Infrastructure.Data;

namespace HandmadeNews.AzureFunc.DbInitialization
{
    [Extension("DbSeed")]
    internal class DbSeedConfigProvider : IExtensionConfigProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbSeedConfigProvider(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

            dbContext.Database.Migrate();
        }
    }
}