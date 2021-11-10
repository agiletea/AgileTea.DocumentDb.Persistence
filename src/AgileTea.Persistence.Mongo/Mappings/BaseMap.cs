using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Common.Records;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace AgileTea.Persistence.Mongo.Mappings
{
    [ExcludeFromCodeCoverage]
    internal static class BaseMap
    {
        public static void MapGuidIndexedEntity()
        {
            BsonClassMap.RegisterClassMap<IndexedEntityBase<Guid>>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id);
            });
        }

        public static void MapObjectIdIndexedEntity()
        {
            BsonClassMap.RegisterClassMap<IndexedEntityBase<ObjectId>>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id);
            });
        }

        public static void MapGuidIndexedRecord()
        {
            BsonClassMap.RegisterClassMap<IndexedRecordBase<Guid>>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id);
            });
        }

        public static void MapObjectIdIndexedRecord()
        {
            BsonClassMap.RegisterClassMap<IndexedRecordBase<ObjectId>>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id);
            });
        }
    }
}
