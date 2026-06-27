using System.Runtime.InteropServices;

namespace Microsoft.EntityFrameworkCore;

public static class ToImmutableArrayAsyncExtensions
{
    public static async Task<ImmutableArray<TSource>> ToImmutableArrayAsync<TSource>(
        this IQueryable<TSource> source, CancellationToken cancellationToken) =>
        ImmutableCollectionsMarshal.AsImmutableArray(
            await EntityFrameworkQueryableExtensions.ToArrayAsync(source, cancellationToken).ConfigureAwait(false));
}