using NorthSouthSystems.MessagePackable;
using System.IO.Hashing;

namespace NorthSouthSystems.Infra;

public class ImmutableBlob
{
    // Copied from NorthSouthSystems.MessagePackable.MessagePack<T>.
    public ImmutableBlob(ImmutableArray<byte> bytes)
    {
        Bytes = Throw.IfDefault(bytes);

        var hasher = new XxHash128();
        hasher.Append(Bytes.AsSpan());
        XxHash128 = hasher.GetCurrentHashAsUInt128();
    }

    private ImmutableBlob(MessagePacked messagePacked)
    {
        Bytes = messagePacked.Bytes;
        XxHash128 = messagePacked.XxHash128;
    }

    public static ImmutableBlob FromMessagePacked(MessagePacked messagePacked) =>
        new(Throw.IfNull(messagePacked));

    public static implicit operator ImmutableBlob(MessagePacked messagePacked) =>
        new(Throw.IfNull(messagePacked));

    public ImmutableArray<byte> Bytes { get; }
    public UInt128 XxHash128 { get; }
}
