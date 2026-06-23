using System.Buffers.Text;
using System.Numerics;

namespace NorthSouthSystems;

public static class UInt128X
{
#pragma warning disable CA1055 // False positive.
    public static string ToBase64UrlBigEndian(this UInt128 uint128) =>
        Base64Url.EncodeToString(ToBytesBigEndian(uint128));
#pragma warning restore

    public static byte[] ToBytesBigEndian(this UInt128 uint128)
    {
        Span<byte> bytes = stackalloc byte[UInt128SizeBytes];
        WriteBigEndian(uint128, bytes);

        return bytes.ToArray();
    }

    private static void WriteBigEndian<T>(T t, Span<byte> bytes)
        where T : struct, IBinaryInteger<T> =>
        t.WriteBigEndian(bytes);

    public static UInt128 FromBytesBigEndian(byte[] bytes)
    {
        Throw.IfNull(bytes);
        Throw.IfNotEqual(bytes.Length, UInt128SizeBytes);

        return ReadBigEndian<UInt128>(bytes);
    }

    private static T ReadBigEndian<T>(byte[] bytes)
        where T : struct, IBinaryInteger<T> =>
        T.ReadBigEndian(bytes, true);

    public const int UInt128SizeBytes = 16;
}