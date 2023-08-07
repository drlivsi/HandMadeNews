using HandmadeNews.AzureFunc.DbInitialization;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(DbInitializationService), "DbSeeder")]
namespace HandmadeNews.AzureFunc.DbInitialization
{
    // https://stackoverflow.com/questions/63017552/ef-core-migrations-in-azure-function-startup
    public class DbInitializationService : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<DbSeedConfigProvider>();
        }
    }
}
