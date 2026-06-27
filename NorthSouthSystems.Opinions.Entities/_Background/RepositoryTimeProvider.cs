using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NorthSouthSystems.Entities;

[ConventionSingleton]
internal sealed class RepositoryTimeProviderWrapper(RepositoryTimeProvider inner) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => Throw.IfNull(inner).GetUtcNow();
}

[ConventionSingleton]
public sealed partial class RepositoryTimeProvider(IOptions<RepositoryTimeProviderOptions> options,
    IServiceScopeFactory serviceScopeFactory, ILogger<RepositoryTimeProvider> logger)
    : BackgroundService
{
    private sealed class RepositoryTime
    {
        internal RepositoryTime(long systemTimestampBefore, DateTimeOffset repositoryTime, long systemTimestampAfter)
        {
            _repositoryUtc = Throw.IfDefault(repositoryTime).ToUniversalTime();
            _systemTimestamp = (systemTimestampBefore + systemTimestampAfter) / 2;

            QueryDuration = systemTimestampAfter - systemTimestampBefore;
        }

        private readonly DateTimeOffset _repositoryUtc;
        private readonly long _systemTimestamp;

        internal long QueryDuration { get; }

        internal DateTimeOffset GetUtcNow() => _repositoryUtc + TimeProvider.System.GetElapsedTime(_systemTimestamp);
    }

    private volatile RepositoryTime _time = null!;

    public DateTimeOffset GetUtcNow() => _time!.GetUtcNow();

    public Task InitializeAsync() =>
        _time is not null
            ? Task.CompletedTask
            : PollAsync(true, CancellationToken.None);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // We lead with Task.Delay because InitializeAsync should have been called immediately prior to
                // BackgroundService.StartAsync (i.e. we very recently finished PollAsync).
                await Task.Delay(options.Value.PollingDelay, stoppingToken).ConfigureAwait(false);
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

            // We'll need to use named services to support multi-server or multi-database.
            var atom = scope.ServiceProvider.GetRequiredService<IAtomicCommandRepository>();

            // We use two iterations in case there is any "warm-up" time for the query (e.g. establishing a connection).
            // We use a third for the sake of a "better result".
            var time = new[]
                {
                    await Query().ConfigureAwait(false),
                    await Query().ConfigureAwait(false),
                    await Query().ConfigureAwait(false)
                }
                .MinBy(r => r.QueryDuration)!;

            async Task<RepositoryTime> Query()
            {
                long systemTimestampBefore = TimeProvider.System.GetTimestamp();
                var repositoryTime = await atom.QuerySysDateTimeOffset(cancellationToken).ConfigureAwait(false);
                long systemTimestampAfter = TimeProvider.System.GetTimestamp();

                return new(systemTimestampBefore, repositoryTime, systemTimestampAfter);
            }

            Interlocked.Exchange(ref _time, time);

            var systemUtcNow = TimeProvider.System.GetUtcNow();
            var repositoryUtcNow = time.GetUtcNow();

            Log.Success(logger, systemUtcNow - repositoryUtcNow, systemUtcNow, repositoryUtcNow);
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

        [LoggerMessage(LogLevel.Information, "Polling success: {delta:c}\n{systemUtcNow:o} system\n{repositoryUtcNow:o} repository")]
        internal static partial void Success(ILogger logger, TimeSpan delta, DateTimeOffset systemUtcNow, DateTimeOffset repositoryUtcNow);

        [LoggerMessage(LogLevel.Warning, "Polling failure")]
        internal static partial void Failure(ILogger logger, Exception exception);
    }
}