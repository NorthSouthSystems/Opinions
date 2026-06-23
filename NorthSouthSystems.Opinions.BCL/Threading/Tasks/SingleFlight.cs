using System.Collections.Concurrent;

namespace NorthSouthSystems.Threading.Tasks;

public sealed class SingleFlight<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _inflights = new();

    // We do not accept a CancellationToken parameter because it is possible that many callers will be awaiting the
    // same Task / factory, and we would not want a single caller's CancellationToken (e.g. RequestAborted) to cancel
    // other awaiting callers (and throw Exceptions for them). We do pass a CancellationToken to the factory should
    // we desire to accept Options dictating when to cancel any and all Tasks / factories.
    //
    // Callers can "await singleFlight.GetOrCreateAsync(...).WaitAsync([their CancellationToken])".
    public Task<TValue> GetOrCreateAsync(TKey key, Func<TKey, CancellationToken, Task<TValue>> factory)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        Throw.IfNull(factory);

        // The "Lazy" pattern here guarantees that exactly one invocation of "factory(...)" occurs; however, per the
        // ConcurrentDictionary docs, it is possible that multiple Lazy instances are constructed (but only one will
        // ever be returned == the "factory(...)" exactly once guarantee).
        var lazy = _inflights.GetOrAdd(
            key,
            k => new(() =>
            {
                var task = factory(k, CancellationToken.None);

                _ = task.ContinueWith(
                    _ => _inflights.TryRemove(k, out var _),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);

                return task;
            }));

        return lazy.Value;
    }
}