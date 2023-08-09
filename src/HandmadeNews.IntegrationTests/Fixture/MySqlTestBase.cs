using System.Data.Common;
using System.Net;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Ductus.FluentDocker.Services.Impl;
using HandmadeNews.Infrastructure.Data;
using HandmadeNews.IntegrationTests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MySql.EntityFrameworkCore.Extensions;
using Polly;

namespace HandmadeNews.IntegrationTests.Fixture
{
    public class MySqlTestBase : DockerComposeTestBase
    {
        private ServiceProvider? _serviceProvider;

        public static TestSettings? TestSettings { get; private set; }

        public ApplicationDbContext? MySqlDbContext { get; private set; }

        protected override ICompositeService Build()
        {
            var composeFile = DockerComposeFileFullPath();

            // Configure & compose Docker composite service
            var dockerComposeFileConfig = new DockerComposeFileConfig
            {
                ComposeFilePath = new List<string> { composeFile },
                ForceRecreate = false,
                RemoveOrphans = true,
                StopOnDispose = true,
            };
            return new DockerComposeCompositeService(DockerHost, dockerComposeFileConfig);
        }

        protected virtual string DockerComposeFileFullPath()
        {
            throw new NotImplementedException();
        }

        protected override void OnContainerBeforeInitialized()
        {
            _serviceProvider = CreateServiceProvider();
            TestSettings = _serviceProvider.GetService<IOptions<TestSettings>>()?.Value;
        }

        protected override void OnContainerInitialized()
        {
            var connectionString = BuildMySqlConnectionString();
            WaitForDatabaseIsReady(connectionString);
            MySqlDbContext = new ApplicationDbContext(CreateDbContextOptions(connectionString));
            MySqlDbContext.Database.Migrate();
        }

        protected override void OnContainerTearDown()
        {
            _serviceProvider = null;
            MySqlDbContext = null;
            TestSettings = null;
        }

        protected virtual string TestSettingsFileFullPath()
        {
            throw new NotImplementedException();
        }

        private DbContextOptions<ApplicationDbContext> CreateDbContextOptions(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseMySQL(connectionString).UseInternalServiceProvider(_serviceProvider);

            return builder.Options;
        }

        private static void WaitForDatabaseIsReady(string connectionString)
        {
            using DbConnection connection = new MySqlConnection(connectionString);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(new TimeSpan(0, 0, 2, 0));
            while (connection.State != System.Data.ConnectionState.Open)
            {
                Policy.Handle<MySqlException>()
                    .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(20)
                    })
                    .ExecuteAsync(connection.OpenAsync);

                if (cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }

            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }

            cancellationTokenSource.Dispose();
        }

        private string BuildMySqlConnectionString()
        {
            IPEndPoint? endPoint = CompositeService?.Containers.FirstOrDefault()
                .ToHostExposedEndpoint(TestSettings?.MySqlPortAndProto);
            if (endPoint == null)
            {
                throw new ApplicationException($"Exposed endpoint is null ({TestSettings?.MySqlPortAndProto})");
            }

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "127.0.0.1",
                Port = (uint)endPoint.Port,
                Database = TestSettings?.MySqlDatabase,
                UserID = TestSettings?.MySqlUser,
                Password = TestSettings?.MySqlPassword,
                PersistSecurityInfo = false,
                MaximumPoolSize = 5,
                MinimumPoolSize = 0,
                ConnectionTimeout = 20,
                ConnectionLifeTime = 300 // 5 minutes
            };
            return builder.ConnectionString;
        }

        private ServiceProvider CreateServiceProvider()
        {
            IServiceCollection? serviceCollection = new ServiceCollection();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(
                    path: TestSettingsFileFullPath(),
                    optional: false,
                    reloadOnChange: true)
                .Build();

            serviceCollection.AddOptions();
            serviceCollection.AddEntityFrameworkMySQL();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.Configure<TestSettings>(configuration.GetSection("TestSettings"));

            return serviceCollection.BuildServiceProvider();
        }
    }
}