using AgileTea.Persistence.Mongo.Context;
using AgileTea.Persistence.Mongo.Enums;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace AgileTea.Persistence.Mongo
{
    public static class MongoDbDriverExtensions
    {
        public static void ConfigureMongo(this IServiceCollection services, MongoOptions? options = null)
        {
            services.AddScoped<IMongoContext, MongoContext>();

            // intialises options with defaults if null
            options ??= new MongoOptions();

            BsonDefaults.GuidRepresentation = options.GuidRepresentation;

            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(options.EnumRepresentation == EnumRepresentation.Numeric ? BsonType.Int32 : BsonType.String),
                new IgnoreExtraElementsConvention(options.IgnoreExtraElementsConvention),
                new IgnoreIfDefaultConvention(options.IgnoreIfDefaultConvention)
            };

            ConventionRegistry.Register("Default Solution Conventions", pack, t => true);
        }
    }
}
