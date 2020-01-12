using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.DependencyInjection;

namespace AgileTea.Persistence.Mongo
{
    [ExcludeFromCodeCoverage]
    public static class MongoDbDriverExtensions
    {
        public static IMongoDbBuilder AddMongo(
            this IServiceCollection services, 
            Action<MongoOptions> options)
        { 
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.AddScoped<IMongoContext, MongoContext>();
            
            // intialises options with defaults if null
            services.Configure(options);

            return new MongoBdBuilder(services);
        }
    }
}
