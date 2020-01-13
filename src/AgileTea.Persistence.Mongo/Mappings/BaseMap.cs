using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Entities;
using MongoDB.Bson.Serialization;

namespace AgileTea.Persistence.Mongo.Mappings
{
    [ExcludeFromCodeCoverage]
    internal static class BaseMap
    {
        public static void Map()
        {
            BsonClassMap.RegisterClassMap<IndexedEntityBase>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id);
            });
        }
    }
}
