using CsCheck;
using NorthSouthSystems.Buffers.Binary;

public class T_BinaryRoundTrip
{
    [Fact]
    public void Bool() => TestBase64(Gen.Bool, BinaryRoundTrip.WriteBase64Bool, BinaryRoundTrip.ReadBase64Bool);

    [Fact]
    public void Byte() => TestBase64(Gen.Byte, BinaryRoundTrip.WriteBase64Byte, BinaryRoundTrip.ReadBase64Byte);

    [Fact]
    public void SByte() => TestBase64(Gen.SByte, BinaryRoundTrip.WriteBase64SByte, BinaryRoundTrip.ReadBase64SByte);

    [Fact]
    public void Short() => TestBase64(Gen.Short, BinaryRoundTrip.WriteBase64Short, BinaryRoundTrip.ReadBase64Short);

    [Fact]
    public void UShort() => TestBase64(Gen.UShort, BinaryRoundTrip.WriteBase64UShort, BinaryRoundTrip.ReadBase64UShort);

    [Fact]
    public void Int() => TestBase64(Gen.Int, BinaryRoundTrip.WriteBase64Int, BinaryRoundTrip.ReadBase64Int);

    [Fact]
    public void UInt() => TestBase64(Gen.UInt, BinaryRoundTrip.WriteBase64UInt, BinaryRoundTrip.ReadBase64UInt);

    [Fact]
    public void Long() => TestBase64(Gen.Long, BinaryRoundTrip.WriteBase64Long, BinaryRoundTrip.ReadBase64Long);

    [Fact]
    public void ULong() => TestBase64(Gen.ULong, BinaryRoundTrip.WriteBase64ULong, BinaryRoundTrip.ReadBase64ULong);

    [Fact]
    public void Double() => TestBase64(Gen.Double, BinaryRoundTrip.WriteBase64Double, BinaryRoundTrip.ReadBase64Double);

    [Fact]
    public void Decimal() => TestBase64(
        Gen.Decimal,
        BinaryRoundTrip.WriteBase64Decimal,
        BinaryRoundTrip.ReadBase64Decimal);

    [Fact]
    public void String() => TestBase64(Gen.String, BinaryRoundTrip.WriteBase64String, BinaryRoundTrip.ReadBase64String);

    private static void TestBase64<T>(Gen<T> gen, Func<T, string> write, Func<string, T> read) =>
        gen.Sample(t => read(write(t)).Should().Be(t), iter: 1_000_000);
}
