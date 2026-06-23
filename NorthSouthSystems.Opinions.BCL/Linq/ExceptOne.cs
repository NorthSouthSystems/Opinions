namespace NorthSouthSystems.Linq;

public static class ExceptOneExtensions
{
    public static IEnumerable<T> ExceptOne<T>(this IEnumerable<T> source, T except, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        foreach (var t in Throw.IfNull(source))
            if (!comparer.Equals(t, except))
                yield return t;
    }
}