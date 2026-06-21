using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace NorthSouthSystems.Buffers.Binary;

public static class BinaryRoundTrip
{
    // ReadBase64 Adapters

    public static bool ReadBase64Bool(string value) => ReadBool(Convert.FromBase64String(value));
    public static byte ReadBase64Byte(string value) => ReadByte(Convert.FromBase64String(value));
    public static sbyte ReadBase64SByte(string value) => ReadSByte(Convert.FromBase64String(value));
    public static short ReadBase64Short(string value) => ReadShort(Convert.FromBase64String(value));
    public static ushort ReadBase64UShort(string value) => ReadUShort(Convert.FromBase64String(value));
    public static int ReadBase64Int(string value) => ReadInt(Convert.FromBase64String(value));
    public static uint ReadBase64UInt(string value) => ReadUInt(Convert.FromBase64String(value));
    public static long ReadBase64Long(string value) => ReadLong(Convert.FromBase64String(value));
    public static ulong ReadBase64ULong(string value) => ReadULong(Convert.FromBase64String(value));
    public static double ReadBase64Double(string value) => ReadDouble(Convert.FromBase64String(value));
    public static decimal ReadBase64Decimal(string value) => ReadDecimal(Convert.FromBase64String(value));
    public static string ReadBase64String(string value) => ReadString(Convert.FromBase64String(value));

    // WriteBase64 Adapters

    public static string WriteBase64Bool(bool value) { string? s = null; WriteBase64Bool(value, b64 => s = b64); return s!; }
    public static string WriteBase64Byte(byte value) { string? s = null; WriteBase64Byte(value, b64 => s = b64); return s!; }
    public static string WriteBase64SByte(sbyte value) { string? s = null; WriteBase64SByte(value, b64 => s = b64); return s!; }
    public static string WriteBase64Short(short value) { string? s = null; WriteBase64Short(value, b64 => s = b64); return s!; }
    public static string WriteBase64UShort(ushort value) { string? s = null; WriteBase64UShort(value, b64 => s = b64); return s!; }
    public static string WriteBase64Int(int value) { string? s = null; WriteBase64Int(value, b64 => s = b64); return s!; }
    public static string WriteBase64UInt(uint value) { string? s = null; WriteBase64UInt(value, b64 => s = b64); return s!; }
    public static string WriteBase64Long(long value) { string? s = null; WriteBase64Long(value, b64 => s = b64); return s!; }
    public static string WriteBase64ULong(ulong value) { string? s = null; WriteBase64ULong(value, b64 => s = b64); return s!; }
    public static string WriteBase64Double(double value) { string? s = null; WriteBase64Double(value, b64 => s = b64); return s!; }
    public static string WriteBase64Decimal(decimal value) { string? s = null; WriteBase64Decimal(value, b64 => s = b64); return s!; }
    public static string WriteBase64String(string value) { string? s = null; WriteBase64String(value, b64 => s = b64); return s!; }

    public static void WriteBase64Bool(bool value, Action<string> writer) => WriteBool(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64Byte(byte value, Action<string> writer) => WriteByte(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64SByte(sbyte value, Action<string> writer) => WriteSByte(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64Short(short value, Action<string> writer) => WriteShort(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64UShort(ushort value, Action<string> writer) => WriteUShort(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64Int(int value, Action<string> writer) => WriteInt(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64UInt(uint value, Action<string> writer) => WriteUInt(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64Long(long value, Action<string> writer) => WriteLong(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64ULong(ulong value, Action<string> writer) => WriteULong(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64Double(double value, Action<string> writer) => WriteDouble(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64Decimal(decimal value, Action<string> writer) => WriteDecimal(value, bytes => writer(Convert.ToBase64String(bytes)));
    public static void WriteBase64String(string value, Action<string> writer) => WriteString(value, bytes => writer(Convert.ToBase64String(bytes)));

    // bool

    public static bool ReadBool(ReadOnlySpan<byte> bytes)
    {
        Throw.IfZero(bytes.Length);
        return bytes[0] > 0;
    }

    public static void WriteBool(bool value, Action<ReadOnlySpan<byte>> writer) =>
        Throw.IfNull(writer)([value ? (byte)1 : (byte)0]);

    // byte

    public static byte ReadByte(ReadOnlySpan<byte> bytes)
    {
        Throw.IfZero(bytes.Length);
        return bytes[0];
    }

    public static void WriteByte(byte value, Action<ReadOnlySpan<byte>> writer) =>
        Throw.IfNull(writer)([value]);

    // sbyte

    public static sbyte ReadSByte(ReadOnlySpan<byte> bytes)
    {
        Throw.IfZero(bytes.Length);
        return unchecked((sbyte)bytes[0]);
    }

    public static void WriteSByte(sbyte value, Action<ReadOnlySpan<byte>> writer) =>
        Throw.IfNull(writer)([(byte)value]);

    // short

    public static short ReadShort(ReadOnlySpan<byte> bytes) =>
        BinaryPrimitives.ReadInt16LittleEndian(bytes);

    public static void WriteShort(short value, Action<ReadOnlySpan<byte>> writer)
    {
        Throw.IfNull(writer);
        Span<byte> bytes = stackalloc byte[2];
        BinaryPrimitives.WriteInt16LittleEndian(bytes, value);
        writer(bytes);
    }

    // ushort

    public static ushort ReadUShort(ReadOnlySpan<byte> bytes) =>
        BinaryPrimitives.ReadUInt16LittleEndian(bytes);

    public static void WriteUShort(ushort value, Action<ReadOnlySpan<byte>> writer)
    {
        Throw.IfNull(writer);
        Span<byte> bytes = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(bytes, value);
        writer(bytes);
    }

    // int

    public static int ReadInt(ReadOnlySpan<byte> bytes) =>
        BinaryPrimitives.ReadInt32LittleEndian(bytes);

    public static void WriteInt(int value, Action<ReadOnlySpan<byte>> writer)
    {
        Throw.IfNull(writer);
        Span<byte> bytes = stackalloc byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
        writer(bytes);
    }

    // uint

    public static uint ReadUInt(ReadOnlySpan<byte> bytes) =>
        BinaryPrimitives.ReadUInt32LittleEndian(bytes);

    public static void WriteUInt(uint value, Action<ReadOnlySpan<byte>> writer)
    {
        Throw.IfNull(writer);
        Span<byte> bytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
        writer(bytes);
    }

    // long

    public static long ReadLong(ReadOnlySpan<byte> bytes) =>
        BinaryPrimitives.ReadInt64LittleEndian(bytes);

    public static void WriteLong(long value, Action<ReadOnlySpan<byte>> writer)
    {
        Throw.IfNull(writer);
        Span<byte> bytes = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(bytes, value);
        writer(bytes);
    }

    // ulong

    public static ulong ReadULong(ReadOnlySpan<byte> bytes) =>
        BinaryPrimitives.ReadUInt64LittleEndian(bytes);

    public static void WriteULong(ulong value, Action<ReadOnlySpan<byte>> writer)
    {
        Throw.IfNull(writer);
        Span<byte> bytes = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes, value);
        writer(bytes);
    }

    // double

    public static double ReadDouble(ReadOnlySpan<byte> bytes) =>
        BitConverter.UInt64BitsToDouble(ReadULong(bytes));

    public static void WriteDouble(double value, Action<ReadOnlySpan<byte>> writer) =>
        WriteULong(BitConverter.DoubleToUInt64Bits(value), writer);

    // decimal

    public static decimal ReadDecimal(ReadOnlySpan<byte> bytes)
    {
        Span<int> parts =
        [
            BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(0, 4)),
            BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(4, 4)),
            BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(8, 4)),
            BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(12, 4))
        ];

        return new(parts);
    }

    public static void WriteDecimal(decimal value, Action<ReadOnlySpan<byte>> writer)
    {
        Throw.IfNull(writer);

        Span<int> parts = stackalloc int[4];
        if (decimal.GetBits(value, parts) != 4)
            throw new UnreachableException();

        Span<byte> bytes = stackalloc byte[16];
        BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(0, 4), parts[0]);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(4, 4), parts[1]);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(8, 4), parts[2]);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(12, 4), parts[3]);

        writer(bytes);
    }

    // string

    public static string ReadString(ReadOnlySpan<byte> bytes) =>
        Encoding.UTF8.GetString(bytes);

    public static void WriteString(string value, Action<ReadOnlySpan<byte>> writer) =>
        Throw.IfNull(writer)(
            Encoding.UTF8.GetBytes(
                Throw.IfNull(value)));
}