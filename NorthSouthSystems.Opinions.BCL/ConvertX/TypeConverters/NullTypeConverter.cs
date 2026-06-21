namespace NorthSouthSystems;

public class NullTypeConverter : ITypeConverter
{
    public void Convert(ConvertTypeRequest request)
    {
        if (Throw.IfNull(request).Value == null && request.ConversionTypeAllowsNull)
            request.Converted(null);
    }
}