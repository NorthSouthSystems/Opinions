namespace NorthSouthSystems;

public class SystemConvertTypeConverter : ITypeConverter
{
    public void Convert(ConvertTypeRequest request)
    {
        // System.Convert.ChangeType requires that value implements IConvertible.
        // https://docs.microsoft.com/en-us/dotnet/api/system.convert.changetype?view=netstandard-2.0
        if ((Throw.IfNull(request).Value == null && !request.ConversionType.IsValueType) || request.Value is IConvertible)
        {
            object? convertedValue = System.Convert.ChangeType(request.Value, request.ConversionType.FlattenGenericNullable(), request.Provider);
            request.Converted(convertedValue);
        }
    }
}