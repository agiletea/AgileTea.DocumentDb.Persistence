using AgileTea.Persistence.Common.Records;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace AgileTea.Persistence.Mongo.Repository
{
    /// <summary>
    /// Base class for creating a repository for a given document type with an ObjectId id
    /// </summary>
    /// <typeparam name="TDocument">The type of document where TDocument is a <see cref="IndexedRecordBase{T}"/></typeparam>
    public class ObjectIdDocumentRecordRepositoryBase<TDocument> : DocumentRepositoryBase<TDocument, ObjectId>
        where TDocument : IndexedRecordBase<ObjectId>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIdDocumentRecordRepositoryBase{TDocument}"/> class.
        /// </summary>
        /// <param name="context">The Mongo context for accessing the document collection and mongo client</param>
        /// <param name="logger">The logger created within the instantiation of specialised class</param>
        protected ObjectIdDocumentRecordRepositoryBase(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
        }
    }
}
