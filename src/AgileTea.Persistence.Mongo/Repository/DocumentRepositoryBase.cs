using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Common.Interfaces;
using AgileTea.Persistence.Common.Repository;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Repository
{
    /// <summary>
    /// Base class for creating a repository for a given document type and id type
    /// </summary>
    /// <typeparam name="TDocument">The type of document where TDocument is a <see cref="IndexedEntityBase{T}"/></typeparam>
    /// <typeparam name="TId">The type of Id used to identify a document</typeparam>
    public abstract class DocumentRepositoryBase<TDocument, TId> : RepositoryBase<TDocument, IMongoContext, TId>
        where TDocument : IIndexedEntity<TId>, new()
    {
        private readonly IMongoContext context;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRepositoryBase{TDocument,TId}"/> class
        /// </summary>
        /// <param name="context">The Mongo context for accessing the document collection and mongo client</param>
        /// <param name="logger">The logger created within the instantiation of specialised class</param>
        protected DocumentRepositoryBase(IMongoContext context, ILogger logger)
            : base(context)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Adds a document to its collection
        /// </summary>
        /// <param name="document">The document to be added</param>
        public override void Add(TDocument document)
        {
            ExecuteDbSetAction((ctx, collection) => ctx.AddCommand(() => collection.InsertOneAsync(document)));
        }

        /// <summary>
        /// Gets a document by its id
        /// </summary>
        /// <param name="id">The id of the document</param>
        /// <returns>The document if found within the collection</returns>
        public override async Task<TDocument> GetByIdAsync(TId id)
        {
            var result = await ExecuteDbSetFuncAsync(collection => collection.FindAsync(
                Builders<TDocument>.Filter.Eq("_id", id)))
                .ConfigureAwait(false);
            return result.SingleOrDefault();
        }

        /// <summary>
        /// Gets a document by its id
        /// </summary>
        /// <param name="id">The id of the document</param>
        /// <returns>The document if found within the collection</returns>
        public override TDocument GetById(TId id)
        {
            var result = ExecuteDbSetFunc(collection => collection.Find(x => x.Id.Equals(id)));
            return result.SingleOrDefault();
        }

        /// <summary>
        /// Gets all documents within a collection
        /// </summary>
        /// <returns>The collection of the given document type</returns>
        public override async Task<IEnumerable<TDocument>> GetAllAsync()
        {
            var result = await ExecuteDbSetFuncAsync(collection => collection
                        .FindAsync(Builders<TDocument>.Filter.Empty))
                        .ConfigureAwait(false);
            return result.ToList();
        }

        /// <summary>
        /// Gets all documents within a collection
        /// </summary>
        /// <returns>The collection of the given document type</returns>
        public override IEnumerable<TDocument> GetAll()
        {
            var result = ExecuteDbSetFunc(collection => collection.Find(Builders<TDocument>.Filter.Empty));
            return result.ToList();
        }

        /// <summary>
        /// Updates the given document within the collection through replacement
        /// </summary>
        /// <param name="document">The document to be used as its replacement</param>
        public override void Update(TDocument document)
        {
            ExecuteDbSetAction((ctx, collection) =>
                ctx.AddCommand(() => collection.ReplaceOneAsync(Builders<TDocument>.Filter.Eq("_id", document.Id), document)));
        }

        /// <summary>
        /// Removes a document from the collection
        /// </summary>
        /// <param name="id">The id of the document to be removed</param>
        public override void Remove(TId id)
        {
            ExecuteDbSetAction((ctx, collection) =>
                ctx.AddCommand(() => collection.DeleteOneAsync(Builders<TDocument>.Filter.Eq("_id", id))));
        }

        /// <summary>
        /// Invokes an action against a given collection
        /// </summary>
        /// <param name="action">The action to invoke</param>
        protected void ExecuteDbSetAction(Action<IMongoContext, IMongoCollection<TDocument>> action)
        {
            var dbSet = GetDbSet();

            // we know the app will throw an exception if the previous statement fails to deliver
            action.Invoke(context, dbSet!);
        }

        /// <summary>
        /// Invokes an asynchronous function against a given collection nd returns the result
        /// </summary>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="TResult">The return value type</typeparam>
        /// <returns>The result of the function</returns>
        protected async Task<TResult> ExecuteDbSetFuncAsync<TResult>(Func<IMongoCollection<TDocument>, Task<TResult>> func)
        {
            var dbSet = GetDbSet();

            // we know the app will throw an exception if the previous statement fails to deliver
            return await func(dbSet!).ConfigureAwait(false);
        }

        /// <summary>
        /// Invokes a function against a given collection nd returns the result
        /// </summary>
        /// <param name="func">The function to invoke</param>
        /// <typeparam name="TResult">The return value type</typeparam>
        /// <returns>The result of the function</returns>
        protected TResult ExecuteDbSetFunc<TResult>(Func<IMongoCollection<TDocument>, TResult> func)
        {
            var dbSet = GetDbSet();

            // we know the app will throw an exception if the previous statement fails to deliver
            return func(dbSet!);
        }

        private IMongoCollection<TDocument> GetDbSet()
        {
            try
            {
                return context.GetCollection<TDocument>(CollectionName);
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
