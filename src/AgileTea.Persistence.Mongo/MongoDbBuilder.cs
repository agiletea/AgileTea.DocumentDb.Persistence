using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Mongo.Enums;
using AgileTea.Persistence.Mongo.Mappings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace AgileTea.Persistence.Mongo
{
    /// <summary>
    /// A builder for the Mongo Db Persistence library to aid configuration and initialisation
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Configuration builder interface, classes and extensions to be used within ConfigureServices. Easier to keep this altogether")]
    public interface IMongoDbBuilder
    {
        // empty on purpose - used for service configuration through extension methods
    }

    /// <summary>
    /// Extension class for adding bson mappings for mongo registration
    /// </summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Configuration builder interface, classes and extensions to be used within ConfigureServices. Easier to keep this altogether")]
    public static class MongoDbBuilderExtensions
    {
        /// <summary>
        /// Adds a list of property mappings for given document type
        /// </summary>
        /// <param name="builder">The <see cref="AgileTea.Persistence.Mongo.IMongoDbBuilder"/> used for registration</param>
        /// <param name="mappings">List of mappings</param>
        /// <typeparam name="T">Document type</typeparam>
        /// <returns>The <see cref="AgileTea.Persistence.Mongo.IMongoDbBuilder"/> instance</returns>
        /// <exception cref="ArgumentNullException">Throws exception if the builder is null</exception>
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

    [ExcludeFromCodeCoverage]
    [SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules",
        "SA1402:File may only contain a single type",
        Justification = "Configuration builder interface, classes and extensions to be used within ConfigureServices. Easier to keep this altogether")]
    internal class MongoBdBuilder : IMongoDbBuilder
    {
        public MongoBdBuilder(IOptions<MongoOptions> options)
        {
            if (options?.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            RegisterMongo(options!.Value);
        }

        private void RegisterMongo(MongoOptions options)
        {
            BsonDefaults.GuidRepresentation = options.GuidRepresentation;

            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(options.EnumRepresentation == EnumRepresentation.Numeric ? BsonType.Int32 : BsonType.String),
                new IgnoreExtraElementsConvention(options.IgnoreExtraElementsConvention),
                new IgnoreIfDefaultConvention(options.IgnoreIfDefaultConvention)
            };

            if (options.UseCamelCaseConvention)
            {
                pack.Add(new CamelCaseElementNameConvention());
            }

            ConventionRegistry.Register("Configured Solution Conventions", pack, t => true);

            BaseMap.Map();
        }
    }
}
