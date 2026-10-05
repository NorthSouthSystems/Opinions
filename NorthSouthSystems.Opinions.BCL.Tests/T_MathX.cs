public class T_MathX
{
    [Theory]
    // Zero
    [InlineData(0, 1, 0)]
    [InlineData(0, 2, 0)]
    [InlineData(0, 0.25, 0)]
    [InlineData(0, 0.5, 0)]
    // Identity
    [InlineData(1, 1, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(-1, 1, -1)]
    [InlineData(-2, 1, -2)]
    [InlineData(0.25, 0.25, 0.25)]
    [InlineData(0.5, 0.25, 0.5)]
    [InlineData(-0.25, 0.25, -0.25)]
    [InlineData(-0.5, 0.25, -0.5)]
    // Up (Positive)
    [InlineData(1, 2, 2)]
    [InlineData(3, 2, 4)]
    [InlineData(2, 3, 3)]
    [InlineData(4, 3, 6)]
    [InlineData(0.125, 0.25, 0.25)]
    [InlineData(0.375, 0.25, 0.5)]
    [InlineData(0.375, 0.5, 0.5)]
    [InlineData(0.625, 0.5, 1)]
    // Up (Negative)
    [InlineData(-1, 2, 0)]
    [InlineData(-3, 2, -2)]
    [InlineData(-2, 3, 0)]
    [InlineData(-4, 3, -3)]
    [InlineData(-0.125, 0.25, 0)]
    [InlineData(-0.375, 0.25, -0.25)]
    [InlineData(-0.375, 0.5, 0)]
    [InlineData(-0.625, 0.5, -0.5)]
    public void CeilingToFactor(decimal value, decimal factor, decimal expectedValue)
    {
        MathX.CeilingToFactor(value, factor).Should().Be(expectedValue);
        MathX.CeilingToFactor((double)value, (double)factor).Should().Be((double)expectedValue);
    }

    [Theory]
    // Zero
    [InlineData(0, 1, 0)]
    [InlineData(0, 2, 0)]
    [InlineData(0, 0.25, 0)]
    [InlineData(0, 0.5, 0)]
    // Identity
    [InlineData(1, 1, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(-1, 1, -1)]
    [InlineData(-2, 1, -2)]
    [InlineData(0.25, 0.25, 0.25)]
    [InlineData(0.5, 0.25, 0.5)]
    [InlineData(-0.25, 0.25, -0.25)]
    [InlineData(-0.5, 0.25, -0.5)]
    // Down (Positive)
    [InlineData(1, 2, 0)]
    [InlineData(3, 2, 2)]
    [InlineData(2, 3, 0)]
    [InlineData(4, 3, 3)]
    [InlineData(0.125, 0.25, 0)]
    [InlineData(0.375, 0.25, 0.25)]
    [InlineData(0.375, 0.5, 0)]
    [InlineData(0.625, 0.5, 0.5)]
    // Down (Negative)
    [InlineData(-1, 2, -2)]
    [InlineData(-3, 2, -4)]
    [InlineData(-2, 3, -3)]
    [InlineData(-4, 3, -6)]
    [InlineData(-0.125, 0.25, -0.25)]
    [InlineData(-0.375, 0.25, -0.5)]
    [InlineData(-0.375, 0.5, -0.5)]
    [InlineData(-0.625, 0.5, -1)]
    public void FloorToFactor(decimal value, decimal factor, decimal expectedValue)
    {
        MathX.FloorToFactor(value, factor).Should().Be(expectedValue);
        MathX.FloorToFactor((double)value, (double)factor).Should().Be((double)expectedValue);
    }

    [Theory]
    // Zero
    [InlineData(0, 1, 0)]
    [InlineData(0, 2, 0)]
    [InlineData(0, 0.25, 0)]
    [InlineData(0, 0.5, 0)]
    // Identity
    [InlineData(1, 1, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(-1, 1, -1)]
    [InlineData(-2, 1, -2)]
    [InlineData(0.25, 0.25, 0.25)]
    [InlineData(0.5, 0.25, 0.5)]
    [InlineData(-0.25, 0.25, -0.25)]
    [InlineData(-0.5, 0.25, -0.5)]
    // Up (Positive)
    [InlineData(1, 2, 2)]
    [InlineData(3, 2, 4)]
    [InlineData(2, 3, 3)]
    [InlineData(0.125, 0.25, 0.25)]
    [InlineData(0.375, 0.25, 0.5)]
    [InlineData(0.375, 0.5, 0.5)]
    // Up (Negative)
    [InlineData(-1, 3, 0)]
    [InlineData(-4, 3, -3)]
    [InlineData(-0.12, 0.25, 0)]
    [InlineData(-0.37, 0.25, -0.25)]
    [InlineData(-0.125, 0.5, 0)]
    [InlineData(-0.625, 0.5, -0.5)]
    // Down (Positive)
    [InlineData(1, 3, 0)]
    [InlineData(4, 3, 3)]
    [InlineData(0.12, 0.25, 0)]
    [InlineData(0.37, 0.25, 0.25)]
    [InlineData(0.125, 0.5, 0)]
    [InlineData(0.625, 0.5, 0.5)]
    // Down (Negative)
    [InlineData(-1, 2, -2)]
    [InlineData(-3, 2, -4)]
    [InlineData(-2, 3, -3)]
    [InlineData(-0.125, 0.25, -0.25)]
    [InlineData(-0.375, 0.25, -0.5)]
    [InlineData(-0.375, 0.5, -0.5)]
    public void RoundToFactor(
        decimal value,
        decimal factor,
        decimal expectedValue,
        MidpointRounding mode = MidpointRounding.AwayFromZero)
    {
        MathX.RoundToFactor(value, factor, mode).Should().Be(expectedValue);
        MathX.RoundToFactor((double)value, (double)factor, mode).Should().Be((double)expectedValue);
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => MathX.RoundToFactor(1.1m, 0m);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("factor");

        act = () => MathX.RoundToFactor(1.1, 0);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("factor");

        act = () => MathX.RoundToFactor(1.1m, -1m);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("factor");

        act = () => MathX.RoundToFactor(1.1, -1);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("factor");

        act = () => MathX.RoundToFactor(3.0m, 2.0m, MidpointRounding.ToEven);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("mode");

        act = () => MathX.RoundToFactor(3.0, 2.0, MidpointRounding.ToEven);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("mode");
    }
}
