using System.Collections.Generic;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Repository
{
    /// <summary>
    /// Base class for creating a repository for a given document type with an ObjectId id
    /// </summary>
    /// <typeparam name="TDocument">The type of document where TDocument is a <see cref="IndexedEntityBase{T}"/></typeparam>
    public class ObjectIdDocumentRepositoryBase<TDocument> : DocumentRepositoryBase<TDocument, ObjectId>
        where TDocument : IndexedEntityBase<ObjectId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIdDocumentRepositoryBase{TDocument}"/> class.
        /// </summary>
        /// <param name="context">The Mongo context for accessing the document collection and mongo client</param>
        /// <param name="logger">The logger created within the instantiation of specialised class</param>
        protected ObjectIdDocumentRepositoryBase(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
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
        public override async Task<TDocument> GetByIdAsync(ObjectId id)
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
        public override TDocument GetById(ObjectId id)
        {
            var result = ExecuteDbSetFunc(collection => collection.Find(doc => doc.Id == id));
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
        public override void Remove(ObjectId id)
        {
            ExecuteDbSetAction((ctx, collection) =>
                ctx.AddCommand(() => collection.DeleteOneAsync(Builders<TDocument>.Filter.Eq("_id", id))));
        }
    }
}
