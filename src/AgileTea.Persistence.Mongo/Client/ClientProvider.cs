using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Mongo.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Client
{
    [ExcludeFromCodeCoverage]
    public sealed class ClientProvider : IClientProvider
    {
        private readonly IAppSettingsProvider appSettingsProvider;
        private readonly ILogger logger;
        private Lazy<IMongoClient>? client;
        private Lazy<IMongoDatabase>? database;

        public ClientProvider(
            IAppSettingsProvider appSettingsProvider,
            ILoggerFactory loggerFactory)
        {
            this.appSettingsProvider = appSettingsProvider;
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
                    return new MongoClient(appSettingsProvider.DbConnection);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to create mongo client and/or database", new
                    {
                        MongoConnection = appSettingsProvider.DbConnection,
                        DatabaseName = appSettingsProvider.DbName
                    });
                    throw;
                }
            });

            database = new Lazy<IMongoDatabase>(() =>
            {
                try
                {
                    logger.LogInformation("Getting Mongo database");
                    return client.Value.GetDatabase(appSettingsProvider.DbName);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to get mongo database", new
                    {
                        MongoConnection = appSettingsProvider.DbConnection,
                        DatabaseName = appSettingsProvider.DbName
                    });
                    throw;
                }
            });
        }
    }
}
