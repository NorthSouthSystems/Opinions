using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace NorthSouthSystems.Entities;

/// <remarks>GetOrCreateAsync uses a single-flight pattern like Nss.Bcl/Threading/Tasks/SingleFlight; however, due to
/// the cache tracking and the extra SetWhenNotNull methods in this class, we cannot delegate to it and instead have
/// copied most of its code here.</remarks>
public abstract class TrackedCache<TValue>(ITrackedCacheVersionProvider versionProvider, ILoggerFactory loggerFactory,
    MemoryCacheOptions? cacheOptions = null)
    : TrackedCache<object, TValue>(versionProvider, loggerFactory, cacheOptions)
{
    protected override sealed object KeySelector(TValue value) => Key;
    private object Key => this;

    private new bool TryGet(object key, out TValue value) =>
        throw new NotSupportedException();

    public bool TryGet(out TValue value) => base.TryGet(Key, out value);

    private new Task<TValue> GetOrCreateAsync(object key, Func<object, CancellationToken, Task<TValue>> factory) =>
        throw new NotSupportedException();

    protected Task<TValue> GetOrCreateAsync(Func<CancellationToken, Task<TValue>> factory) =>
        base.GetOrCreateAsync(Key, (_, ct) => factory(ct));
}

public abstract class TrackedCache<TKey, TValue>(ITrackedCacheVersionProvider versionProvider, ILoggerFactory loggerFactory,
    MemoryCacheOptions? cacheOptions = null)
    : IDisposable
    where TKey : notnull
{
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
            _cache.Dispose();
    }

    private readonly Lock _lock = new();
    private int _version = int.MinValue;

    private readonly MemoryCache _cache = new(cacheOptions ?? new(), loggerFactory);
    private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _createsInProgress = new();

    protected abstract TKey KeySelector(TValue value);

    protected virtual MemoryCacheEntryOptions GetEntryOptions(TKey key, TValue value) => new();

    public bool TryGet(TKey key, out TValue value)
    {
        value = default!;

        int entryVersion;
        lock (_lock) { entryVersion = _version; }

        if (!_cache.TryGetValue(key, out object? untyped))
            return false;

        int trackedVersion = Throw.IfNull(versionProvider).GetVersion(GetType());

        if (entryVersion == trackedVersion)
        {
            value = untyped is TValue typed
                ? typed
                : default!;

            return true;
        }

        lock (_lock)
        {
            if (_version < trackedVersion)
            {
                _version = trackedVersion;
                _cache.Clear();
                _createsInProgress.Clear();
            }
        }

        return false;
    }

    // We do not accept a CancellationToken parameter because it is possible that many callers will be awaiting the
    // same Task / factory, and we would not want a single caller's CancellationToken (e.g. RequestAborted) to cancel
    // other awaiting callers (and throw Exceptions for them). We do pass a CancellationToken to the factory should
    // we desire to accept Options dictating when to cancel any and all Tasks / factories.
    //
    // Callers can "await cache.GetOrCreateAsync(...).WaitAsync([their CancellationToken])".
    protected Task<TValue> GetOrCreateAsync(TKey key, Func<TKey, CancellationToken, Task<TValue>> factory)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        Throw.IfNull(factory);

        if (TryGet(key, out var existing))
            return Task.FromResult(existing);

        // The "Lazy" pattern here guarantees that exactly one invocation of CreateAsync occurs; however, per the
        // ConcurrentDictionary docs, it is possible that multiple Lazy instances are constructed (but only one will
        // ever be returned == the CreateAsync exactly once guarantee).
        //
        // This pattern offers performance benefits and prevents cache stampedes.
        var create = _createsInProgress.GetOrAdd(
            key,
            k => new(() =>
            {
                var task = CreateAsync(k, factory, CancellationToken.None);

                _ = task.ContinueWith(
                    _ => _createsInProgress.TryRemove(k, out var _),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);

                return task;
            }));

        return create.Value;
    }

    // We must use factories for [Create|SetWhenNotNull]Async rather than taking Task parameters directly because we
    // must capture _version prior to TValue creation work. If we took the Task parameter directly, it could contain
    // work done for a previous version of this TrackedCache.
    //
    // NOTE : Any changes to this method should be reviewed for the VERY similar [Create|SetWhenNotNull]Async methods.
    private async Task<TValue> CreateAsync(TKey key, Func<TKey, CancellationToken, Task<TValue>> factory,
        CancellationToken cancellationToken)
    {
        // We only cache if we are on the "entry version". If a potential race-condition occurred, we remove our key.
        int entryVersion;
        lock (_lock) { entryVersion = _version; }

        var value = await factory(key, cancellationToken).ConfigureAwait(false);

        lock (_lock)
        {
            if (_version != entryVersion)
                return value;
        }

        if (value is not null && !key.Equals(KeySelector(value)))
            throw new ArgumentOutOfRangeException(nameof(key), string.Create(InvariantCulture, $"Must match the {nameof(KeySelector)} result."));

        _cache.Set(key, value, GetEntryOptions(key, value));

        // There could be some thrashing in VERY rare cases when a TrackedCacheVersion is changed while calling Set;
        // however, this is the most correct option and worth any performance costs in those rare (possibly never) cases.
        bool remove;

        lock (_lock) { remove = _version != entryVersion; }

        if (remove)
            _cache.Remove(key);

        return value;
    }

    // See comment above CreateAsync.
    //
    // NOTE : Any changes to this method should be reviewed for the VERY similar [Create|SetWhenNotNull]Async methods.
    public async Task<TValue> SetWhenNotNullAsync(Func<Task<TValue>> factory)
    {
        Throw.IfNull(factory);

        // We only cache if we are on the "entry version". If a potential race-condition occurred, we remove our key.
        int entryVersion;
        lock (_lock) { entryVersion = _version; }

        var value = await factory().ConfigureAwait(false);

        lock (_lock)
        {
            if (_version != entryVersion)
                return value;
        }

        if (value is null)
            return value;

        var key = KeySelector(value);

        _cache.Set(key, value, GetEntryOptions(key, value));

        // There could be some thrashing in VERY rare cases when a TrackedCacheVersion is changed; however, this is the
        // most correct and worth any performance costs in those rare (possibly never) cases.
        bool remove;

        lock (_lock) { remove = _version != entryVersion; }

        if (remove)
            _cache.Remove(key);

        return value;
    }

    // See comment above CreateAsync.
    //
    // NOTE : Any changes to this method should be reviewed for the VERY similar [Create|SetWhenNotNull]Async methods.
    public async Task<ImmutableArray<TValue>> SetWhenNotNullAsync(Func<Task<ImmutableArray<TValue>>> factory)
    {
        Throw.IfNull(factory);

        // We only cache if we are on the "entry version". If a potential race-condition occurred, we remove our key.
        int entryVersion;
        lock (_lock) { entryVersion = _version; }

        var values = await factory().ConfigureAwait(false);

        lock (_lock)
        {
            if (_version != entryVersion)
                return values;
        }

        var keyValues = values
            .Where(v => v is not null)
            .Select(v => KeyValuePair.Create(KeySelector(v), v))
            .ToImmutableArray();

        foreach (var kvp in keyValues)
            _cache.Set(kvp.Key, kvp.Value, GetEntryOptions(kvp.Key, kvp.Value));

        // There could be some thrashing in VERY rare cases when a TrackedCacheVersion is changed; however, this is the
        // most correct and worth any performance costs in those rare (possibly never) cases.
        bool remove;

        lock (_lock) { remove = _version != entryVersion; }

        if (remove)
            foreach (var kvp in keyValues)
                _cache.Remove(kvp.Key);

        return values;
    }
}