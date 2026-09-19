public class T_DateTimeOffsetUtcDateTimeConverter
{
    [Fact]
    public void Convert()
    {
        var converter = new DateTimeOffsetUtcDateTimeConverter();
        var toDateTime = converter.ConvertToProviderExpression.Compile();
        var fromDateTime = converter.ConvertFromProviderExpression.Compile();

        var now = DateTimeOffset.UtcNow;

        // Sanity
        now.DateTime.Kind.Should().Be(DateTimeKind.Unspecified);
        now.UtcDateTime.Kind.Should().Be(DateTimeKind.Utc);
        now.LocalDateTime.Kind.Should().Be(DateTimeKind.Local);

        toDateTime.Invoke(now).Should().Be(now.UtcDateTime);

        fromDateTime.Invoke(now.DateTime).Should().Be(now);
        fromDateTime.Invoke(now.UtcDateTime).Should().Be(now);

        Action act = () => fromDateTime.Invoke(now.LocalDateTime);
        act.Should().ThrowExactly<NotSupportedException>();
    }
}
