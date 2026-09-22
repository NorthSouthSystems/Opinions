namespace NorthSouthSystems.Infra;

public sealed record ImmutableBlobPickled(ImmutableBlob Blob, ImmutableArray<byte> Pickled);
