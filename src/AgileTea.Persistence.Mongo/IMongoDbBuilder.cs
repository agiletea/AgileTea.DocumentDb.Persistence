using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Mongo.Enums;
using AgileTea.Persistence.Mongo.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace AgileTea.Persistence.Mongo
{
    public interface IMongoDbBuilder
    {
        void RegisterMongo();
    }

    [ExcludeFromCodeCoverage]
    internal class MongoBdBuilder : IMongoDbBuilder
    {
        private readonly IOptionsMonitor<MongoOptions> options;

        public MongoBdBuilder(IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            options = builder.GetService<IOptionsMonitor<MongoOptions>>();
        }

        public void RegisterMongo()
        {
            BsonDefaults.GuidRepresentation = options.CurrentValue.GuidRepresentation;

            BaseMap.Map();

            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(options.CurrentValue.EnumRepresentation == EnumRepresentation.Numeric ? BsonType.Int32 : BsonType.String),
                new IgnoreExtraElementsConvention(options.CurrentValue.IgnoreExtraElementsConvention),
                new IgnoreIfDefaultConvention(options.CurrentValue.IgnoreIfDefaultConvention)
            };

            ConventionRegistry.Register("Configured Solution Conventions", pack, t => true);
        }
    }

    [ExcludeFromCodeCoverage]
    internal static class MongoDbBuilderExtensions
    {

        public static IMongoDbBuilder AddMappings<T>(this IMongoDbBuilder builder, params Action<BsonClassMap<T>>[] mappings)
            where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            BsonClassMap.RegisterClassMap<T>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                foreach (var mapping in mappings)
                {
                    mapping.Invoke(map);
                }
            });

            return builder;
        }
    }
}
