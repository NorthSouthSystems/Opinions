using NorthSouthSystems.Buffers.Binary;
using System.IO.Hashing;
using System.Runtime.InteropServices;

namespace NorthSouthSystems.IO.Hashing;

public static class XxHash128X
{
    // We leverage our BinaryRoundTrip helper for "stable" binary representations of all primitives
    // except for string. BinaryRoundTrip uses Encoding.UTF8.GetBytes for string, which is likely
    // slower than the very simple method below (possibly a premature optimization). However, both
    // Encoding.UTF8.GetBytes and MemoryMarshal.AsBytes return no bytes for string.Empty, thereby
    // causing it not to affect the hash. Our method "solves" this potential problem.

#pragma warning disable CA1062 // A NullReferenceException here would be "expected" by the caller.
    public static void Append(this XxHash128 hasher, bool value) => BinaryRoundTrip.WriteBool(value, hasher.Append);
    public static void Append(this XxHash128 hasher, byte value) => BinaryRoundTrip.WriteByte(value, hasher.Append);
    public static void Append(this XxHash128 hasher, sbyte value) => BinaryRoundTrip.WriteSByte(value, hasher.Append);
    public static void Append(this XxHash128 hasher, short value) => BinaryRoundTrip.WriteShort(value, hasher.Append);
    public static void Append(this XxHash128 hasher, ushort value) => BinaryRoundTrip.WriteUShort(value, hasher.Append);
    public static void Append(this XxHash128 hasher, int value) => BinaryRoundTrip.WriteInt(value, hasher.Append);
    public static void Append(this XxHash128 hasher, uint value) => BinaryRoundTrip.WriteUInt(value, hasher.Append);
    public static void Append(this XxHash128 hasher, long value) => BinaryRoundTrip.WriteLong(value, hasher.Append);
    public static void Append(this XxHash128 hasher, ulong value) => BinaryRoundTrip.WriteULong(value, hasher.Append);
    public static void Append(this XxHash128 hasher, double value) => BinaryRoundTrip.WriteDouble(AppendNormalize(value), hasher.Append);
    public static void Append(this XxHash128 hasher, decimal value) => BinaryRoundTrip.WriteDecimal(value, hasher.Append);
#pragma warning restore

    public static void Append(this XxHash128 hasher, string value)
    {
        Throw.IfNull(hasher);
        Throw.IfNull(value);

        // This line results in 0 bytes for string.Empty, which does not cause the hash to change.
        // Since we don't allow null values, this will not allow the hash to distinguish between
        // null and string.Empty without intervention. We choose to hash a single sentinel byte
        // for string.Empty, which because we are using UTF16, this sentinel cannot collide with
        // any character that is potentially hashed afterwards (it can collide with other inputs though).
        var utf16 = MemoryMarshal.AsBytes(value.AsSpan());

        if (utf16.Length > 0)
            hasher.Append(utf16);
        else
            hasher.Append(_stringEmptySentinel);
    }

    // Our intuition is that this is better than either 0 or 255 for reducing potential collisions.
    private const byte _stringEmptySentinel = 0b_0101_0101;

    private static double AppendNormalize(double value)
    {
        // double has multiple bit patterns for NaN and both -0.0 and +0.0; we normalize them.
        if (double.IsNaN(value)) return double.NaN;
        else if (value == double.NegativeZero) return 0.0;
        else return value;
    }
}