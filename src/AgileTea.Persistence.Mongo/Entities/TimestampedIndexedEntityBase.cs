using System;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgileTea.Persistence.Mongo.Entities
{
    /// <summary>
    /// Document entity extended from <see cref="ObjectIdIndexedEntityBase"/> to provide Timestamp properties
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TimestampedIndexedEntityBase : ObjectIdIndexedEntityBase
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets or sets the Bson timestamp
        /// </summary>
        [BsonElement("ts")]
        public BsonTimestamp Timestamp { get; set; }

        /// <summary>
        /// Gets the created date using the ObjectId
        /// </summary>
        [BsonIgnore]
        public DateTime? Created => Id.CreationTime;

        /// <summary>
        /// Gets the last updated timestamp if the timestamp has been set
        /// </summary>
        [BsonIgnore]
        public DateTime? LastUpdated => GetTimestamp();

        /// <summary>
        /// Sets the Timestamp field
        /// </summary>
        /// <returns>The calculated timestamp value based on current date and time</returns>
        public BsonTimestamp SetTimestamp()
        {
            var target = DateTime.UtcNow;
            var diff = target.ToUniversalTime() - UnixEpoch;
            var seconds = (diff.TotalMilliseconds + 18000000) / 1000;
            return new BsonTimestamp((int)seconds, 1);
        }

        private DateTime? GetTimestamp()
        {
            return Timestamp == null ? (DateTime?)null : UnixEpoch.AddSeconds(Timestamp!.Timestamp - 18000);
        }
    }
}
