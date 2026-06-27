using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NorthSouthSystems.Entities;

public static class ProgramNorthSouthSystemsEntities
{
    public static IServiceCollection AddNorthSouthSystemsEntitiesDefaults(this IServiceCollection services)
    {
        Throw.IfNull(services).ConventionScanAssembly(Assembly.GetExecutingAssembly());

        services.AddSingleton<TimeProvider>(sp => sp.GetRequiredService<RepositoryTimeProviderWrapper>());

        return services;
    }

    public static async Task InitNorthSouthSystemsEntitiesDefaults(this IServiceProvider serviceProvider, bool excludeRepository = false)
    {
        if (!excludeRepository)
        {
            var timeProvider = serviceProvider.GetRequiredService<RepositoryTimeProvider>();
            await timeProvider.InitializeAsync().ConfigureAwait(false);
            TimeProviderContext.SetRoot(serviceProvider.GetRequiredService<RepositoryTimeProviderWrapper>());

            var trackedCacheVersionProvider = serviceProvider.GetRequiredService<RepositoryTrackedCacheVersionProvider>();
            await trackedCacheVersionProvider.InitializeAsync().ConfigureAwait(false);
        }
    }
}