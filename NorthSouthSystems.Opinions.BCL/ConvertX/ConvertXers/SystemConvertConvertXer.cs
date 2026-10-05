namespace NorthSouthSystems;

public class SystemConvertConvertXer : IConvertXer
{
    public void Convert(ConvertXRequest request)
    {
        // System.Convert.ChangeType requires that value implements IConvertible.
        // https://docs.microsoft.com/en-us/dotnet/api/system.convert.changetype?view=netstandard-2.0
        if ((Throw.IfNull(request).Value == null && !request.ConversionTypeFlattened.IsValueType)
            || request.Value is IConvertible)
        {
            object? convertedValue =
                System.Convert.ChangeType(request.Value, request.ConversionTypeFlattened, request.Culture);
            request.Converted(convertedValue);
        }
    }
}
