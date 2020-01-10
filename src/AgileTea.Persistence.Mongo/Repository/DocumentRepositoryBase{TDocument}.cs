using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Common.Repository;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Repository
{
    public class DocumentRepositoryBase<TDocument> : RepositoryBase<TDocument, IMongoContext>
        where TDocument : IndexedEntityBase
    {
        private readonly IMongoContext context;
        private readonly ILogger logger;

        protected DocumentRepositoryBase(IMongoContext context, ILogger logger) 
            : base(context, logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public override void Add(TDocument document)
        {
            ExecuteDbSetAction((ctx, collection) => ctx.AddCommand(() => collection.InsertOneAsync(document)));
        }

        public override async Task<TDocument> GetById(Guid id)
        {
            var result = await ExecuteDbSetFunc(collection =>
                    collection.FindAsync(Builders<TDocument>.Filter.Eq("_id", id))).ConfigureAwait(false);
            return result.SingleOrDefault();
        }

        public override async Task<IEnumerable<TDocument>> GetAll()
        {
            var result = await ExecuteDbSetFunc(collection => 
                collection.FindAsync(Builders<TDocument>.Filter.Empty)).ConfigureAwait(false);
                
            return result.ToList();
        }

        public override void Update(TDocument document)
        {
            ExecuteDbSetAction((ctx, collection) =>
                ctx.AddCommand(() => collection.ReplaceOneAsync(Builders<TDocument>.Filter.Eq("_id", document.Id), document)));
        }

        public override void Remove(Guid id)
        {
            ExecuteDbSetAction((ctx, collection) =>
                ctx.AddCommand(() => collection.DeleteOneAsync(Builders<TDocument>.Filter.Eq("_id", id))));
        }

        private void ExecuteDbSetAction(Action<IMongoContext, IMongoCollection<TDocument>> action)
        {
            var dbSet = GetDbSet();

            // we know the app will throw an exception if the previous statement fails to deliver
            action.Invoke(context, dbSet!);
        }

        private async Task<TResult> ExecuteDbSetFunc<TResult>(Func<IMongoCollection<TDocument>, Task<TResult>> func)
        {
            var dbSet = GetDbSet();

            // we know the app will throw an exception if the previous statement fails to deliver
            return await func(dbSet!).ConfigureAwait(false);
        }

        private IMongoCollection<TDocument> GetDbSet()
        {
            try
            {
                return context.GetCollection<TDocument>(typeof(TDocument).Name);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get collection for entity", new
                {
                    EntityType = typeof(TDocument).Name
                });

                throw;
            }
        }
    }
}
