using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Entities;
using MongoDB.Bson;

namespace AgileTea.Persistence.Mongo.Entities
{
    /// <summary>
    /// Mongo specific indexed entity using <see cref="ObjectId"/> in place of a GUID to aid with creation time stamps
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ObjectIdIndexedEntityBase : IndexedEntityBase<ObjectId>
    {
    }
}
