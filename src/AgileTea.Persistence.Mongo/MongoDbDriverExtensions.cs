using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common;
using AgileTea.Persistence.Mongo.Client;
using AgileTea.Persistence.Mongo.Context;
using AgileTea.Persistence.Mongo.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

            services.Configure(options);
            using var serviceProvider = services.BuildServiceProvider();
            var builder = new MongoBdBuilder(serviceProvider.GetRequiredService<IOptions<MongoOptions>>());

            services.RegisterCommonServices();
            services.AddSingleton<IClientProvider, ClientProvider>();
            services.AddScoped<IMongoContext, MongoContext>();

            return builder;
        }

        /// <summary>
        /// Sets up services for the Mongo Db Persistence library with a given mapping class
        /// </summary>
        /// <param name="services">The <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></param>
        /// <param name="options">The mongo options to be used for configuration</param>
        /// <typeparam name="TMappings">Mapping class declaring the property mappings for the collection based on <see cref="AgileTea.Persistence.Mongo.Mappings.MongoDbMappings"/></typeparam>
        /// <exception cref="ArgumentNullException">Thrown if options are null</exception>
        public static void AddMongo<TMappings>(this IServiceCollection services,
            Action<MongoOptions> options)
            where TMappings : MongoDbMappings, new()
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.RegisterCommonServices();
            services.Configure(options);
            services.AddSingleton<ClientProvider>();
            services.AddSingleton<IClientProvider>(ctx =>
            {
                var mappings = new TMappings();
                mappings.ApplyOptions(ctx.GetRequiredService<IOptions<MongoOptions>>());
                mappings.InitialiseMappings();
                return ctx.GetRequiredService<ClientProvider>();
            });
            services.AddScoped<IMongoContext, MongoContext>();
        }
    }
}
