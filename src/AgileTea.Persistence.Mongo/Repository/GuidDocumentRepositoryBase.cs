using System;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;

namespace AgileTea.Persistence.Mongo.Repository
{
    /// <summary>
    /// Base class for creating a repository for a given document type
    /// </summary>
    /// <typeparam name="TDocument">The type of document where TDocument is a <see cref="IndexedEntityBase{T}"/></typeparam>
    public class GuidDocumentRepositoryBase<TDocument> : DocumentRepositoryBase<TDocument, Guid>
        where TDocument : IndexedEntityBase<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidDocumentRepositoryBase{TDocument}"/> class.
        /// </summary>
        /// <param name="context">The Mongo context for accessing the document collection and mongo client</param>
        /// <param name="logger">The logger created within the instantiation of specialised class</param>
        protected GuidDocumentRepositoryBase(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
        }
    }
}
