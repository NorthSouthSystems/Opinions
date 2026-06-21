using System.Globalization;

namespace NorthSouthSystems;

public class ConvertTypeRequest
{
    internal ConvertTypeRequest(object? value, Type conversionType, IFormatProvider provider)
    {
        Value = value;
        ConversionType = conversionType ?? throw new ArgumentNullException(nameof(conversionType));
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public object? Value { get; }
    public Type ConversionType { get; }
    public IFormatProvider Provider { get; }

    public bool ConversionTypeAllowsNull => !ConversionType.IsValueType || ConversionType.IsGenericNullable();

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

        _exceptions ??= new();
        _exceptions.Add(exception);
    }

    internal Exception ExceptionToThrow()
    {
        string message = string.Create(CultureInfo.InvariantCulture, $"{Value?.GetType().FullName} : {ConversionType.FullName}");

        if (_exceptions == null)
            return new NotSupportedException(message);
        else if (_exceptions.Count == 1)
            return new InvalidCastException(message, _exceptions.Single());
        else
            return new AggregateException(message, _exceptions);
    }
}