namespace NorthSouthSystems.Infra;

public interface IImmutableBlobCache
{
    Task<ImmutableBlobPickled?> TryReadAsync(UInt128 xxHash128, CancellationToken cancellationToken);

    Task<ImmutableArray<ImmutableBlobPickled?>> TryReadAsync(
        ImmutableArray<UInt128> xxHash128s,
        CancellationToken cancellationToken);

    Task WriteAsync(ImmutableBlobPickled blobPickled, CancellationToken cancellationToken);

    Task WriteAsync(ImmutableArray<ImmutableBlobPickled> blobPickleds, CancellationToken cancellationToken);
}
