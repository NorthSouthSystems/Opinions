public class T_Throw
{
    [Fact]
    public void GreaterThanEnum()
    {
        Throw.IfGreaterThanEnum(SomeEnum.Zero, SomeEnum.Zero);
        Throw.IfGreaterThanEnum(SomeEnum.Zero, SomeEnum.One);
        Throw.IfGreaterThanEnum(SomeEnum.One, SomeEnum.Two);

        Action action = () => Throw.IfGreaterThanEnum(SomeEnum.One, SomeEnum.Zero);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfGreaterThanEnum(SomeEnum.Two, SomeEnum.One);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GreaterThanOrEqualEnum()
    {
        Throw.IfGreaterThanOrEqualEnum(SomeEnum.Zero, SomeEnum.One);
        Throw.IfGreaterThanOrEqualEnum(SomeEnum.One, SomeEnum.Two);

        Action action = () => Throw.IfGreaterThanOrEqualEnum(SomeEnum.Zero, SomeEnum.Zero);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfGreaterThanOrEqualEnum(SomeEnum.One, SomeEnum.Zero);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void LessThanEnum()
    {
        Throw.IfLessThanEnum(SomeEnum.Zero, SomeEnum.Zero);
        Throw.IfLessThanEnum(SomeEnum.One, SomeEnum.Zero);
        Throw.IfLessThanEnum(SomeEnum.Two, SomeEnum.One);

        Action action = () => Throw.IfLessThanEnum(SomeEnum.Zero, SomeEnum.One);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfLessThanEnum(SomeEnum.One, SomeEnum.Two);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void LessThanOrEqualEnum()
    {
        Throw.IfLessThanOrEqualEnum(SomeEnum.One, SomeEnum.Zero);
        Throw.IfLessThanOrEqualEnum(SomeEnum.Two, SomeEnum.One);

        Action action = () => Throw.IfLessThanOrEqualEnum(SomeEnum.One, SomeEnum.One);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfLessThanOrEqualEnum(SomeEnum.One, SomeEnum.Two);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Between()
    {
        Throw.IfBetween(0, 1, 2);
        Throw.IfBetween(3, 1, 2);
        Throw.IfBetween(0, 1, 3);
        Throw.IfBetween(4, 1, 3);

        Action action = () => Throw.IfBetween(1, 1, 2);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfBetween(2, 1, 2);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfBetween(1, 1, 3);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfBetween(2, 1, 3);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfBetween(3, 1, 3);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NotBetween()
    {
        Throw.IfNotBetween(1, 1, 2);
        Throw.IfNotBetween(2, 1, 2);
        Throw.IfNotBetween(1, 1, 3);
        Throw.IfNotBetween(2, 1, 3);
        Throw.IfNotBetween(3, 1, 3);

        Action action = () => Throw.IfNotBetween(0, 1, 2);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfNotBetween(3, 1, 2);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfNotBetween(0, 1, 3);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfNotBetween(4, 1, 3);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void BetweenEnum()
    {
        Throw.IfBetweenEnum(SomeEnum.Zero, SomeEnum.One, SomeEnum.Two);
        Throw.IfBetweenEnum(SomeEnum.Three, SomeEnum.One, SomeEnum.Two);
        Throw.IfBetweenEnum(SomeEnum.Zero, SomeEnum.One, SomeEnum.Three);
        Throw.IfBetweenEnum(SomeEnum.Four, SomeEnum.One, SomeEnum.Three);

        Action action = () => Throw.IfBetweenEnum(SomeEnum.One, SomeEnum.One, SomeEnum.Two);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfBetweenEnum(SomeEnum.Two, SomeEnum.One, SomeEnum.Two);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfBetweenEnum(SomeEnum.One, SomeEnum.One, SomeEnum.Three);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfBetweenEnum(SomeEnum.Two, SomeEnum.One, SomeEnum.Three);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfBetweenEnum(SomeEnum.Three, SomeEnum.One, SomeEnum.Three);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NotBetweenEnum()
    {
        Throw.IfNotBetweenEnum(SomeEnum.One, SomeEnum.One, SomeEnum.Two);
        Throw.IfNotBetweenEnum(SomeEnum.Two, SomeEnum.One, SomeEnum.Two);
        Throw.IfNotBetweenEnum(SomeEnum.One, SomeEnum.One, SomeEnum.Three);
        Throw.IfNotBetweenEnum(SomeEnum.Two, SomeEnum.One, SomeEnum.Three);
        Throw.IfNotBetweenEnum(SomeEnum.Three, SomeEnum.One, SomeEnum.Three);

        Action action = () => Throw.IfNotBetweenEnum(SomeEnum.Zero, SomeEnum.One, SomeEnum.Two);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfNotBetweenEnum(SomeEnum.Three, SomeEnum.One, SomeEnum.Two);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfNotBetweenEnum(SomeEnum.Zero, SomeEnum.One, SomeEnum.Three);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();

        action = () => Throw.IfNotBetweenEnum(SomeEnum.Four, SomeEnum.One, SomeEnum.Three);
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    private enum SomeEnum
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
}