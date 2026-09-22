using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NorthSouthSystems.Infra;

public static class ProgramNorthSouthSystemsInfraAbstractions
{
    public static IServiceCollection AddNorthSouthSystemsInfraAbstractionsDefaults(this IServiceCollection services)
    {
        Throw.IfNull(services).ConventionScanAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    public static Task InitNorthSouthSystemsInfraAbstractionsDefaultsAsync(this IServiceProvider serviceProvider) =>
        Task.CompletedTask;
}
