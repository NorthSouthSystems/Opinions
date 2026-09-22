using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Reflection;

namespace NorthSouthSystems.Infra;

public static class ProgramNorthSouthSystemsInfra
{
    public static IServiceCollection AddNorthSouthSystemsInfraDefaults(this IServiceCollection services)
    {
        Throw.IfNull(services).ConventionScanAssembly(Assembly.GetExecutingAssembly());

        services.AddConventionOptions<AzureBlobContainerClientOptions>(nameof(AzureImmutableBlobStorage));

        services.AddKeyedSingleton<BlobContainerClient>(
            KeyedService.AnyKey,
            (sp, key) =>
            {
                // IOptions doesn't support named options; IOptionsSnapshot does but is Scoped so cannot be used here.
                var options =
                    sp.GetRequiredService<IOptionsMonitor<AzureBlobContainerClientOptions>>()
                        .Get(key?.ToString());

                return new(options.ConnectionStringNoContainerName, options.ContainerName);
            });

        services.AddConventionOptions<RedisClientOptions>(nameof(RedisImmutableBlobCache));

        services.AddKeyedSingleton<IDatabase>(
            KeyedService.AnyKey,
            (sp, key) =>
            {
                // IOptions doesn't support named options; IOptionsSnapshot does but is Scoped so cannot be used here.
                var options = sp.GetRequiredService<IOptionsMonitor<RedisClientOptions>>().Get(key?.ToString());
                var config = ConfigurationOptions.Parse(options.ConfigurationString!);
                var mux = ConnectionMultiplexer.Connect(config);

                return mux.GetDatabase(options.Database);
            });

        return services;
    }

    public static Task InitNorthSouthSystemsInfraDefaultsAsync(this IServiceProvider serviceProvider) =>
        Task.CompletedTask;
}
