using System.Runtime.InteropServices;

namespace NorthSouthSystems.Linq;

public static class ToImmutableArrayAsyncExtensions
{
    public static async ValueTask<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default)
    {
        var array = await Throw.IfNull(source).ToArrayAsync(cancellationToken).ConfigureAwait(false);

        return ImmutableCollectionsMarshal.AsImmutableArray(array);
    }
}