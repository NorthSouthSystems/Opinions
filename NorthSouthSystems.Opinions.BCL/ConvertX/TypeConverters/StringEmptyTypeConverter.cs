namespace NorthSouthSystems;

public class StringEmptyTypeConverter : ITypeConverter
{
    public void Convert(ConvertTypeRequest request)
    {
        if (Throw.IfNull(request).Value is string { Length: 0 })
        {
            if (request.ConversionType == typeof(string))
                request.Converted(request.Value);
            else if (request.ConversionTypeAllowsNull)
                request.Converted(null);
        }
    }
}