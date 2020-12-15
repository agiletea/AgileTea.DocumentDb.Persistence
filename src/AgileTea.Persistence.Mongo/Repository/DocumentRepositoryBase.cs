using System;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Entities;
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
        where TDocument : IndexedEntityBase<TId>
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
