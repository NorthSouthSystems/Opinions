namespace NorthSouthSystems;

public class ConvertXRequest
{
    internal ConvertXRequest(object? value, Type conversionType, CultureInfo culture)
    {
        Value = value;
        ConversionTypeAllowsNull = !Throw.IfNull(conversionType).IsValueType || conversionType.IsGenericNullable();
        ConversionTypeFlattened = conversionType.FlattenGenericNullable();
        Culture = Throw.IfNull(culture);
    }

    public object? Value { get; }
    public bool ConversionTypeAllowsNull { get; }
    public Type ConversionTypeFlattened { get; }
    public CultureInfo Culture { get; }

    public bool IsConverted { get; private set; }
    public object? ConvertedValue { get; private set; }

    public void Converted(object? convertedValue)
    {
        IsConverted = true;
        ConvertedValue = convertedValue;
    }

    // Don't create unneccessary garbage; instantiate when the first Exception is added.
    private List<Exception>? _exceptions;

    internal void Exception(Exception exception)
    {
        Throw.IfNull(exception);

        _exceptions ??= [];
        _exceptions.Add(exception);
    }

    internal Exception ExceptionToThrow()
    {
        bool genericNullable = ConversionTypeAllowsNull && ConversionTypeFlattened.IsValueType;

        string conversionTypeName = string.Create(
            InvariantCulture,
            $"{(genericNullable ? "System.Nullable<" : string.Empty)}{ConversionTypeFlattened.FullName}{(genericNullable ? ">" : string.Empty)}");

        string message = string.Create(InvariantCulture, $"{Value?.GetType().FullName} : {conversionTypeName}");

        if (_exceptions == null)
            return new NotSupportedException(message);
        else if (_exceptions.Count == 1)
            return new InvalidCastException(message, _exceptions.Single());
        else
            return new AggregateException(message, _exceptions);
    }
}
