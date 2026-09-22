using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using StackExchange.Redis;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NorthSouthSystems.Infra;

[ScanRegisterSingleton]
public sealed class RedisImmutableBlobCache([FromKeyedServices(nameof(RedisImmutableBlobCache))] IDatabase database)
    : IImmutableBlobCache
{
    public async Task<ImmutableBlobPickled?> TryReadAsync(
        UInt128 xxHash128,
        CancellationToken cancellationToken)
    {
        Throw.IfNull(database);

        var key = (RedisKey)xxHash128.ToBytesBigEndian();
        var value = await database.StringGetAsync(key).ConfigureAwait(false);

        return RedisValueToImmutableBlobPickled(xxHash128, value);
    }

    public async Task<ImmutableArray<ImmutableBlobPickled?>> TryReadAsync(
        ImmutableArray<UInt128> xxHash128s,
        CancellationToken cancellationToken)
    {
        Throw.IfNull(database);

        var keys = xxHash128s
            .Select(hash => (RedisKey)hash.ToBytesBigEndian())
            .ToArray();

        var values = await database.StringGetAsync(keys).ConfigureAwait(false);

        return [.. xxHash128s.EquiZip(values, RedisValueToImmutableBlobPickled)];
    }

    public Task WriteAsync(ImmutableBlobPickled blobPickled, CancellationToken cancellationToken)
    {
        Throw.IfNull(database);
        Throw.IfNull(blobPickled);

        var kvp = ImmutableBlobPickledToRedisKeyValue(blobPickled);

        return database.StringSetAsync(kvp.Key, kvp.Value);
    }

    public Task WriteAsync(ImmutableArray<ImmutableBlobPickled> blobPickleds, CancellationToken cancellationToken)
    {
        Throw.IfNull(database);

        var kvps = blobPickleds
            .Select(ImmutableBlobPickledToRedisKeyValue)
            .ToArray();

        return database.StringSetAsync(kvps);
    }

    private static ImmutableBlobPickled? RedisValueToImmutableBlobPickled(UInt128 xxHash128, RedisValue value)
    {
        if (!value.HasValue)
            return null;

        var pickled = ImmutableCollectionsMarshal.AsImmutableArray((byte[])value!);
        var bytes = CompressionPickler.Unpickle(pickled);
        var blob = new ImmutableBlob(bytes);

        return xxHash128 == blob.XxHash128
            ? new(blob, pickled)
            : throw new UnreachableException("Hash mismatch.");
    }

    private static KeyValuePair<RedisKey, RedisValue> ImmutableBlobPickledToRedisKeyValue(
        ImmutableBlobPickled blobPickled) =>
        KeyValuePair.Create(
            (RedisKey)blobPickled.Blob.XxHash128.ToBytesBigEndian(),
            (RedisValue)blobPickled.Pickled.AsMemory());
}
