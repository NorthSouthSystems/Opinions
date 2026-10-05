using System.ComponentModel;

namespace NorthSouthSystems;

public class SystemTypeConverterConvertXer : IConvertXer
{
    public void Convert(ConvertXRequest request)
    {
        if (Throw.IfNull(request).Value is null)
            return;

        var valueType = request.Value!.GetType();

        var valueTypeConverter = TypeDescriptor.GetConverter(valueType);

        if (valueTypeConverter.CanConvertTo(request.ConversionTypeFlattened))
        {
            request.Converted(
                valueTypeConverter.ConvertTo(null, request.Culture, request.Value, request.ConversionTypeFlattened));
            return;
        }

        var conversionTypeConverter = TypeDescriptor.GetConverter(request.ConversionTypeFlattened);

        if (conversionTypeConverter.CanConvertFrom(valueType))
        {
            request.Converted(conversionTypeConverter.ConvertFrom(null, request.Culture, request.Value!));
            return;
        }
    }
}
