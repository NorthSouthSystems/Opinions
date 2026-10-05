internal static class T_ConvertXer
{
    internal static ConvertXRequest Convert(
        IConvertXer converter,
        object value,
        Type conversionType,
        CultureInfo culture = null)
    {
        var request = new ConvertXRequest(value, conversionType, culture ?? CurrentCulture);
        converter.Convert(request);

        return request;
    }
}
