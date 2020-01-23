using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Mongo.Enums;
using MongoDB.Bson;

namespace AgileTea.Persistence.Mongo
{
    /// <summary>
    /// Options for connecting to a mongo database including specific conventions
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MongoOptions
    {
        /// <summary>
        /// Gets or sets the mongo database connection. Cannot be null or empty
        /// </summary>
        public string DbConnection { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the Mongo database. Cannot be null or empty
        /// </summary>
        public string DbName { get; set; } = default!;

        /// <summary>
        /// Gets or set whether to support Cosmos Db. Default is false
        /// Note: setting this to true will switch off support for transactions within the Mongo Context
        /// </summary>
        public bool CanSupportCosmos { get; set; } = false;

        /// <summary>
        /// Gets or sets how Guids are represented in string format. Defaults to <see cref="GuidRepresentation.CSharpLegacy"/>
        /// </summary>
        public GuidRepresentation GuidRepresentation { get; set; } = GuidRepresentation.CSharpLegacy;

        /// <summary>
        /// Gets or sets whether to ignore extra elements. Defaults to <c>true</c>
        /// </summary>
        public bool IgnoreExtraElementsConvention { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to ignore if default. Defaults to <c>true</c>
        /// </summary>
        public bool IgnoreIfDefaultConvention { get; set; } = true;

        /// <summary>
        /// Gets or sets how enums are returned from the store. Defaults to <see cref="string"/>
        /// </summary>
        public EnumRepresentation EnumRepresentation { get; set; } = EnumRepresentation.String;

        /// <summary>
        /// Gets or sets whether to use camelCaseConvention. Defaults to <c>true</c>
        /// </summary>
        public bool UseCamelCaseConvention { get; set; } = true;
    }
}
