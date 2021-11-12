using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Records;
using MongoDB.Bson;

namespace AgileTea.Persistence.Mongo.Records
{
    /// <summary>
    /// Mongo specific indexed record using <see cref="ObjectId"/> in place of a GUID to aid with creation time stamps
    /// </summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Not with Positional Records")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "Not with Positional Records")]
    public abstract record ObjectIdIndexedRecordBase(ObjectId Id) : IndexedRecordBase<ObjectId>(Id);
}
