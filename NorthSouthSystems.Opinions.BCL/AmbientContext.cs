namespace NorthSouthSystems;

#pragma warning disable CA1000 // In this case, the only public members are static, so call site ambiguity is unlikely.
public abstract class AmbientContext<T>
    where T : class
{
    public static void SetRoot(T t)
    {
        if (_root is not null)
            throw new NotSupportedException(string.Create(InvariantCulture, $"{nameof(SetRoot)} may only be called once."));

        _root = Throw.IfNull(t);
    }

    public static T Current =>
        CurrentAsyncLocal.Value
        ?? _root
        ?? throw new InvalidOperationException(
            string.Create(InvariantCulture, $"{nameof(AmbientContext<>)} not initialized. Call {nameof(SetRoot)} during application startup."));

    private static T? _root;
    private static readonly AsyncLocal<T?> CurrentAsyncLocal = new();

    public static IDisposable OverrideCurrent(T? t)
    {
        var previous = CurrentAsyncLocal.Value;
        CurrentAsyncLocal.Value = t;

        return new OverrideCookie(previous);
    }

    private sealed class OverrideCookie(T? previous) : IDisposable
    {
        public void Dispose() { CurrentAsyncLocal.Value = previous; }
    }
}
#pragma warning restore