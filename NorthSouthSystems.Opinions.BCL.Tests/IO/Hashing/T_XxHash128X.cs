using CsCheck;
using NorthSouthSystems.IO.Hashing;
using System.IO.Hashing;

public class T_XxHash128X
{
    [Fact]
    public void Bool() => Test(Gen.Bool, XxHash128X.Append);

    [Fact]
    public void Byte() => Test(Gen.Byte, XxHash128X.Append);

    [Fact]
    public void SByte() => Test(Gen.SByte, XxHash128X.Append);

    [Fact]
    public void Short() => Test(Gen.Short, XxHash128X.Append);

    [Fact]
    public void UShort() => Test(Gen.UShort, XxHash128X.Append);

    [Fact]
    public void Int() => Test(Gen.Int, XxHash128X.Append);

    [Fact]
    public void UInt() => Test(Gen.UInt, XxHash128X.Append);

    [Fact]
    public void Long() => Test(Gen.Long, XxHash128X.Append);

    [Fact]
    public void ULong() => Test(Gen.ULong, XxHash128X.Append);

    [Fact]
    public void Double() => Test(Gen.Double, XxHash128X.Append);

    [Fact]
    public void Decimal() => Test(Gen.Decimal, XxHash128X.Append);

    [Fact]
    public void String() => Test(Gen.String, XxHash128X.Append);

    private static void Test<T>(Gen<T> gen, Action<XxHash128, T> hashAppend) =>
        gen.Sample(
            t =>
            {
                var hasher = new XxHash128();
                hasher.GetCurrentHashAsUInt128().Should().Be(_hashInitialExpected);

                hashAppend(hasher, t);
                hasher.GetCurrentHashAsUInt128().Should().NotBe(_hashInitialExpected);
            },
            iter: 1_000_000);

    private static readonly UInt128 _hashInitialExpected = new XxHash128().GetCurrentHashAsUInt128();
}
