using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace NorthSouthSystems.Entities;

[ConventionSingleton]
public sealed partial class RepositoryTrackedCacheVersionProvider(
    IOptions<RepositoryTrackedCacheVersionProviderOptions> options, IServiceScopeFactory serviceScopeFactory,
    ILogger<RepositoryTrackedCacheVersionProvider> logger)
    : BackgroundService, ITrackedCacheVersionProvider
{
    private readonly ConcurrentDictionary<string, int> _versionsByTypeName = [];

    public int GetVersion(Type trackedCacheType) => _versionsByTypeName[Throw.IfNull(trackedCacheType).Name];

    public Task InitializeAsync() => PollAsync(true, CancellationToken.None);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // We lead with Task.Delay because InitializeAsync should have been called immediately prior to
                // BackgroundService.StartAsync (i.e. we very recently finished PollAsync).
                await Task.Delay(Throw.IfNull(options).Value.PollingDelay, stoppingToken).ConfigureAwait(false);
                await PollAsync(false, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
        }
    }

    private async Task PollAsync(bool isInitialize, CancellationToken cancellationToken)
    {
        try
        {
            Log.InProgress(logger);

            var scope = Throw.IfNull(serviceScopeFactory).CreateAsyncScope();
            await using var _ = scope.ConfigureAwait(false);

            var read = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository>();

            var versions = await read.TrackedCacheVersions
                .ToImmutableArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var version in versions)
            {
                _versionsByTypeName.AddOrUpdate(
                    version.TypeName,
                    version.Updated.Version,
                    (_, currentVersion) => Math.Max(currentVersion, version.Updated.Version));
            }

            Log.Success(logger, versions.Length);
        }
#pragma warning disable CA1031 // Exceptions shall not escape and "kill" this BackgroundService.
        catch (Exception exception)
        {
            Log.Failure(logger, exception);

            if (isInitialize)
                throw;
        }
#pragma warning restore
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Polling in progress")]
        internal static partial void InProgress(ILogger logger);

        [LoggerMessage(LogLevel.Information, "Polling success: {count}")]
        internal static partial void Success(ILogger logger, int count);

        [LoggerMessage(LogLevel.Warning, "Polling failure")]
        internal static partial void Failure(ILogger logger, Exception exception);
    }
}