using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Mongo.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace AgileTea.Persistence.Mongo.Mappings
{
    /// <summary>
    /// Base class to apply provided mongo options and applies mapping of properties
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class MongoDbMappings
    {
        internal void ApplyOptions(IOptions<MongoOptions> options)
        {
            if (options?.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            BsonDefaults.GuidRepresentation = options!.Value.GuidRepresentation;

            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(options!.Value.EnumRepresentation == EnumRepresentation.Numeric ? BsonType.Int32 : BsonType.String),
                new IgnoreExtraElementsConvention(options!.Value.IgnoreExtraElementsConvention),
                new IgnoreIfDefaultConvention(options!.Value.IgnoreIfDefaultConvention)
            };

            if (options!.Value.UseCamelCaseConvention)
            {
                pack.Add(new CamelCaseElementNameConvention());
            }

            ConventionRegistry.Register("Configured Solution Conventions", pack, t => true);

            BaseMap.Map();
        }

        /// <summary>
        /// Abstract method placeholder to enforce the initialisation of entity mappings
        /// </summary>
        protected internal abstract void InitialiseMappings();

        /// <summary>
        /// Helper method to enable easy registration of entity mappings
        /// </summary>
        /// <param name="mappings">Collection of mappings to apply</param>
        /// <typeparam name="TDocument">The class to be mapped</typeparam>
        protected void AddMappings<TDocument>(params Action<BsonClassMap<TDocument>>[] mappings)
            where TDocument : IndexedEntityBase
        {
            BsonClassMap.RegisterClassMap<TDocument>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                foreach (var mapping in mappings)
                {
                    mapping.Invoke(map);
                }
            });
        }
    }
}
