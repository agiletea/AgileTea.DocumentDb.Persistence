using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

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
    }
}
