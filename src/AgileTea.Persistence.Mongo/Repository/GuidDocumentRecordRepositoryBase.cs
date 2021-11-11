using System;
using AgileTea.Persistence.Common.Records;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;

namespace AgileTea.Persistence.Mongo.Repository
{
    /// <summary>
    /// Base class for creating a repository for a given document type
    /// </summary>
    /// <typeparam name="TDocument">The type of document where TDocument is a <see cref="IndexedRecordBase{T}"/></typeparam>
    public class GuidDocumentRecordRepositoryBase<TDocument> : DocumentRepositoryBase<TDocument, Guid>
        where TDocument : IndexedRecordBase<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidDocumentRecordRepositoryBase{TDocument}"/> class.
        /// </summary>
        /// <param name="context">The Mongo context for accessing the document collection and mongo client</param>
        /// <param name="logger">The logger created within the instantiation of specialised class</param>
        protected GuidDocumentRecordRepositoryBase(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
        }
    }
}
