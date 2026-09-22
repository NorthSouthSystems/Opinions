using MoreLinq;
using System.Collections.Frozen;

namespace NorthSouthSystems.Infra;

// We don't also "Write-Through" because IImmutableBlobCache supports one-call-write-many but IImmutableBlobStorage
// does not. Whenever we write to IImmutableBlobStorage, we can optionally write to IImmutableBlobCache (it is not
// required for correctness), and can optionally leverage the IImmutableBlobCache one-call-write-many performance.
[ScanRegisterSingleton]
public sealed class ReadThroughImmutableBlobCache(IImmutableBlobCache blobCache, IImmutableBlobStorage blobStorage)
{
    public async Task<IReadOnlyDictionary<UInt128, ImmutableBlobPickled>> ReadThroughAsync(
        ImmutableArray<UInt128> xxHash128s,
        CancellationToken cancellationToken)
    {
        Throw.IfNull(blobCache);
        Throw.IfNull(blobStorage);
        Throw.IfDefault(xxHash128s);

        if (xxHash128s.Length == 0)
            return FrozenDictionary<UInt128, ImmutableBlobPickled>.Empty;

        var blobPickledsByXxHash128 = await TryGetBlobCacheAsync(xxHash128s, cancellationToken).ConfigureAwait(false);
        await ReadBlobStorageAsync(blobPickledsByXxHash128, cancellationToken).ConfigureAwait(false);

        return blobPickledsByXxHash128!;
    }

    private async Task<Dictionary<UInt128, ImmutableBlobPickled?>> TryGetBlobCacheAsync(
        ImmutableArray<UInt128> xxHash128s,
        CancellationToken cancellationToken)
    {
        var blobPickleds = await blobCache.TryReadAsync(xxHash128s, cancellationToken).ConfigureAwait(false);

        return xxHash128s.EquiZip(blobPickleds, KeyValuePair.Create).ToDictionary();
    }

    private async Task ReadBlobStorageAsync(
        Dictionary<UInt128, ImmutableBlobPickled?> blobPickledsByXxHash128,
        CancellationToken cancellationToken)
    {
        var reads = blobPickledsByXxHash128.Where(kvp => kvp.Value is null)
            .Select(kvp => blobStorage.ReadAsync(kvp.Key, cancellationToken));

        var blobPickleds = await Task.WhenAll(reads).ConfigureAwait(false);

        if (blobPickleds.Length == 0)
            return;

        // If we've made it this far, these blobs are not in the blobCache (barring a race), so fire-and-forget write.
        _ = blobCache.WriteAsync([.. blobPickleds], CancellationToken.None);

        foreach (var blobPickled in blobPickleds)
            blobPickledsByXxHash128[blobPickled.Blob.XxHash128] = blobPickled;
    }
}
