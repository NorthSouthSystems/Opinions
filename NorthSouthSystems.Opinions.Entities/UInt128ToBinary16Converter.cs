using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NorthSouthSystems.Entities;

internal sealed class UInt128ToBinary16Converter()
    : ValueConverter<UInt128, byte[]>(
        u => ToBytesBigEndian(u),
        b => FromBytesBigEndian(b),
        new(size: UInt128X.UInt128SizeBytes))
{
    private static byte[] ToBytesBigEndian(UInt128 uint128) => UInt128X.ToBytesBigEndian(uint128);
    private static UInt128 FromBytesBigEndian(byte[] bytes) => UInt128X.FromBytesBigEndian(bytes);
}