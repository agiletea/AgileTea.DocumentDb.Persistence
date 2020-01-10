using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace AgileTea.Persistence.Mongo.Configuration
{
    [ExcludeFromCodeCoverage]
    internal class AppSettingsProvider : IAppSettingsProvider
    {
        public AppSettingsProvider(IConfiguration configuration)
        {
            DbConnection = configuration["MongoSettings:Connection"];
            DbName = configuration["MongoSettings:DatabaseName"];
        }

        public string DbConnection { get; }
        public string DbName { get; }
    }
}