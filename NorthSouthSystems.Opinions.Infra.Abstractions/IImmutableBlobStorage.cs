namespace NorthSouthSystems.Infra;

public sealed record ImmutableBlobStorageWriteResult(bool Wrote, ImmutableBlobPickled BlobPickled);

public interface IImmutableBlobStorage
{
    Task<ImmutableBlobPickled> ReadAsync(UInt128 xxHash128, CancellationToken cancellationToken);

    Task<ImmutableBlobStorageWriteResult> WriteIfNotExistsAsync(
        ImmutableBlob blob,
        IDictionary<string, string>? tags,
        CancellationToken cancellationToken);
}
