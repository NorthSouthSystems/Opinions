using K4os.Compression.LZ4;

namespace NorthSouthSystems.Infra;

[ScanRegisterConventionOptions]
public sealed class CompressionPicklerOptions
{
    // This can safely be changed in the future (assuming performance testing, correctness testing, etc.) because the
    // whole raison d'être of the Pickler is to embed (and unembed) this information in each pickled bytes.
    public BlobCompressionType CompressionTypeFuture { get; set; } = BlobCompressionType.LZ4;

    // This can safely be changed when using K4os.Compression.LZ4 because decompression does not need to know it.
    //
    // All of our ImmutableBlobs are write once (i.e. compress once) read many (i.e. decompress many), and because LZ4
    // decompression speed does not vary with compression level, there is a strong incentive for us to use the highest
    // compression level: reduced blob and Redis costs; faster blob and Redis retrieval, possibly slightly faster
    // decompression due to smaller buffers. With that said, testing with MessagePacked blobs showed only approximately
    // a 1% size reduction for a 10x compression slowdown when going from L03_HC to L12_MAX. I hypothesize this is
    // because MessagePack is already a "tight" binary format.
    //
    // ReSharper disable once InconsistentNaming
    public LZ4Level CompressionTypeLZ4Level { get; set; } = LZ4Level.L03_HC;
}

[ScanRegisterSingleton]
public sealed class CompressionPicklerOptionsValidator : AbstractValidator<CompressionPicklerOptions>
{
    public CompressionPicklerOptionsValidator()
    {
        RuleFor(x => x.CompressionTypeFuture)
            .IsInEnum()
            .NotEqual(BlobCompressionType._Undefined);

        RuleFor(x => x.CompressionTypeLZ4Level).IsInEnum();
    }
}
