public class T_ConvertX_Culture
{
    [Fact]
    public void Culture()
    {
        // https://stackoverflow.com/a/8492527
        var negativeBangCulture = (CultureInfo)InvariantCulture.Clone();
        negativeBangCulture.NumberFormat.NegativeSign = "!";

        IsConvertedTrue("1", InvariantCulture, 1);
        IsConvertedTrue("1", negativeBangCulture, 1);

        IsConvertedFalse("!1", InvariantCulture);
        IsConvertedTrue("!1", negativeBangCulture, -1);

        IsConvertedTrue("-1", InvariantCulture, -1);
        IsConvertedFalse("-1", negativeBangCulture);

        static void IsConvertedTrue(string value, CultureInfo culture, int expectedConvertedValue)
        {
            ConvertX.Default.ConvertType<int>(value, culture).Should().Be(expectedConvertedValue);
            ConvertX.Default.ConvertType(value, typeof(int), culture).Should().Be(expectedConvertedValue);

            int convertedValueInt;
            object convertedValueObject;

            ConvertX.Default.TryConvertType<int>(value, culture, out convertedValueInt).Should().BeTrue();
            convertedValueInt.Should().Be(expectedConvertedValue);

            ConvertX.Default.TryConvertType(value, typeof(int), culture, out convertedValueObject).Should().BeTrue();
            convertedValueObject.Should().Be(expectedConvertedValue);
        }

        static void IsConvertedFalse(string value, CultureInfo culture)
        {
            Action act;

            act = () => ConvertX.Default.ConvertType<int>(value, culture);
            act.Should()
                .ThrowExactly<AggregateException>()
                .WithInnerExceptionExactly<ArgumentException>()
                .WithInnerExceptionExactly<FormatException>();

            act = () => ConvertX.Default.ConvertType(value, typeof(int), culture);
            act.Should()
                .ThrowExactly<AggregateException>()
                .WithInnerExceptionExactly<ArgumentException>()
                .WithInnerExceptionExactly<FormatException>();

            int convertedValueInt;
            object convertedValueObject;

            ConvertX.Default.TryConvertType<int>(value, culture, out convertedValueInt).Should().BeFalse();
            convertedValueInt.Should().Be(0);

            ConvertX.Default.TryConvertType(value, typeof(int), culture, out convertedValueObject).Should().BeFalse();
            convertedValueObject.Should().Be(0);
        }
    }
}