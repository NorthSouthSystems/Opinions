namespace NorthSouthSystems.Collections.Generic;

public static class ReadOnlyCollectionWrapperExtensions
{
    public static IReadOnlyCollection<T> AsReadOnlyCollectionWrapper<T>(this ICollection<T> collection) =>
        Throw.IfNull(collection) as IReadOnlyCollection<T> ?? new ReadOnlyCollectionWrapper<T>(collection);

    private sealed class ReadOnlyCollectionWrapper<T>(ICollection<T> inner) : IReadOnlyCollection<T>
    {
        public int Count => inner.Count;

        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}