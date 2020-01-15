using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace AgileTea.Persistence.Common
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void RegisterCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        }
    }
}
