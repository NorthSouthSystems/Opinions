namespace NorthSouthSystems.Linq;

public static class ReverseNoBufferExtensions
{
    public static IEnumerable<T> ReverseNoBuffer<T>(this IReadOnlyList<T> source) =>
        ReverseNoBufferCore(Throw.IfNull(source));

    private static IEnumerable<T> ReverseNoBufferCore<T>(IReadOnlyList<T> source)
    {
        for (int i = source.Count - 1; i >= 0; i--)
            yield return source[i];
    }
}