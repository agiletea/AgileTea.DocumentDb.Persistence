using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Mongo.Client;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Context
{
    internal class MongoContext : IMongoContext
    {
        private readonly IClientProvider clientProvider;
        private readonly List<Func<Task>> commands;
        private readonly ILogger logger;
        private IClientSessionHandle session;

        public MongoContext(
            IClientProvider clientProvider,
            ILoggerFactory loggerFactory)
        {
            this.clientProvider = clientProvider;
            logger = loggerFactory.CreateLogger(typeof(MongoContext).FullName);

            // every command will be stored and it'll be processed at SaveChanges
            commands = new List<Func<Task>>();
        }

        public async Task<int> SaveChangesAsync()
        {
            CheckMongo();

            // we know the app will throw an exception if the previous statement fails to deliver
            using (session = await clientProvider.Client!.StartSessionAsync())
            {
                session.StartTransaction();

                var commandTasks = commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await session.CommitTransactionAsync();
            }

            return commands.Count;
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>(string name)
            where TDocument : IndexedEntityBase
        {
            CheckMongo();

            // we know the app will throw an exception if the previous statement fails to deliver
            return clientProvider.Database!.GetCollection<TDocument>(name);
        }

        public void AddCommand(Func<Task> func)
        {
            commands.Add(func);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                session?.Dispose();
            }
        }

        private void CheckMongo()
        {
            if (clientProvider.Client == null)
            {
                logger.LogError("Mongo check failed. Client is null");
                throw new InvalidOperationException("Mongo check failed. Client is null");
            }

            if (clientProvider.Database == null)
            {
                logger.LogError("Mongo check failed. Database is null");
                throw new InvalidOperationException("Mongo check failed. Database is null");
            }
        }
    }
}