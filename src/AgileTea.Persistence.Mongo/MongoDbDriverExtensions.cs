using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common;
using AgileTea.Persistence.Mongo.Client;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.DependencyInjection;

namespace AgileTea.Persistence.Mongo
{
    /// <summary>
    /// <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> extensions for registering the services
    /// specific to the Mongo Db Persistence library
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class MongoDbDriverExtensions
    {
        /// <summary>
        /// Sets up services for the Mongo Db Persistence library
        /// </summary>
        /// <param name="services">The <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></param>
        /// <param name="options">The mongo options to be used for configuration</param>
        /// <returns>A <see cref="AgileTea.Persistence.Mongo.IMongoDbBuilder"/> to allow for registering of class mappings</returns>
        /// <exception cref="ArgumentNullException">Thrown if options are null</exception>
        public static IMongoDbBuilder AddMongo(
            this IServiceCollection services,
            Action<MongoOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.RegisterCommonServices();
            services.AddSingleton<IClientProvider, ClientProvider>();
            services.AddScoped<IMongoContext, MongoContext>();
            services.Configure(options);

            return new MongoBdBuilder(services);
        }
    }
}
