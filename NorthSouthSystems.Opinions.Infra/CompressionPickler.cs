using K4os.Compression.LZ4;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace NorthSouthSystems.Infra;

#pragma warning disable CA1028 // Byte represents that this is the leading byte of the Pickle.
#pragma warning disable CA1707 // Underscore is used for _Undefined to visually "stand-out".
// WARNING - DO NOT CHANGE THESE NAMES. JsonSerializer is configured to use them exactly.
// WARNING - DO NOT CHANGE THESE VALUES. MessagePackSerializer is configured to use them exactly.
// WARNING - DO NOT CHANGE THESE VALUES. They serve as the leading byte of the pickle.
public enum BlobCompressionType : byte
{
    // ReSharper disable once InconsistentNaming
    _Undefined = 0,

    // ReSharper disable once InconsistentNaming
    LZ4 = 1
}
#pragma warning restore

[ScanRegisterSingleton]
public sealed class CompressionPickler(IOptions<CompressionPicklerOptions> options)
{
    public ImmutableArray<byte> Pickle(ImmutableArray<byte> uncompressed)
    {
        Throw.IfDefault(uncompressed);

        var compressionType = Throw.IfNull(options).Value.CompressionTypeFuture;

        // TODO : Is there a way to perform the "inner pickle" while leaving room for the leading byte so that we don't
        // need to allocate a second byte[]?
        byte[] compressed = compressionType switch
        {
            BlobCompressionType._Undefined => throw new InvalidOperationException(),

            // LZ4 must do its own Pickle'ing because it needs to prepend details about its compression (e.g. size).
            BlobCompressionType.LZ4 => LZ4Pickler.Pickle(uncompressed.AsSpan(), options.Value.CompressionTypeLZ4Level),

            _ => throw new NotSupportedException(compressionType.ToString())
        };

        byte[] pickled = new byte[compressed.Length + 1];
        pickled[0] = (byte)compressionType;
        compressed.CopyTo(pickled.AsSpan(1));

        return ImmutableCollectionsMarshal.AsImmutableArray(pickled);
    }

    public static ImmutableArray<byte> Unpickle(ImmutableArray<byte> pickled)
    {
        Throw.IfDefault(pickled);
        Throw.IfLessThan(pickled.Length, 1);

        var compressionType = (BlobCompressionType)pickled[0];
        var compressed = pickled.AsSpan()[1..];

        byte[] uncompressed = compressionType switch
        {
            BlobCompressionType._Undefined => throw new InvalidOperationException(),

            // LZ4 must do its own Pickle'ing because it needs to prepend details about its compression (e.g. size).
            BlobCompressionType.LZ4 => LZ4Pickler.Unpickle(compressed),

            _ => throw new NotSupportedException(compressionType.ToString())
        };

        return ImmutableCollectionsMarshal.AsImmutableArray(uncompressed);
    }
}
