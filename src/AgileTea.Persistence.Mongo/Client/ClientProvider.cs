using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Client
{
    [ExcludeFromCodeCoverage]
    internal sealed class ClientProvider : IClientProvider
    {
        private readonly IOptionsMonitor<MongoOptions> options;
        private readonly ILogger logger;
        private Lazy<IMongoClient> client;
        private Lazy<IMongoDatabase> database;

        public ClientProvider(
            IOptionsMonitor<MongoOptions> options,
            ILoggerFactory loggerFactory)
        {
            this.options = options;
            logger = loggerFactory.CreateLogger<ClientProvider>();

            ConfigureConnection();
        }

        public IMongoClient Client => client!.Value;

        public IMongoDatabase Database => database!.Value;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Client?.Cluster.Dispose();
            }
        }

        private void ConfigureConnection()
        {
            client = new Lazy<IMongoClient>(() =>
            {
                try
                {
                    logger.LogInformation("Initiating Mongo database connection");
                    return new MongoClient(options.CurrentValue.DbConnection);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to create mongo client and/or database", new
                    {
                        MongoConnection = options.CurrentValue.DbConnection,
                        DatabaseName = options.CurrentValue.DbName
                    });
                    throw;
                }
            });

            database = new Lazy<IMongoDatabase>(() =>
            {
                try
                {
                    logger.LogInformation("Getting Mongo database");
                    return client.Value.GetDatabase(options.CurrentValue.DbName);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to get mongo database", new
                    {
                        MongoConnection = options.CurrentValue.DbConnection,
                        DatabaseName = options.CurrentValue.DbName
                    });
                    throw;
                }
            });
        }
    }
}
