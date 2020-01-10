﻿using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Mongo.Enums;
using MongoDB.Bson;

namespace AgileTea.Persistence.Mongo
{
    [ExcludeFromCodeCoverage]
    public class MongoOptions
    {
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
        /// Gets or sets how enums are returned from the store. Defaults to <see cref="EnumRepresentation.String"/>
        /// </summary>
        public EnumRepresentation EnumRepresentation { get; set; } = EnumRepresentation.String;
    }
}
