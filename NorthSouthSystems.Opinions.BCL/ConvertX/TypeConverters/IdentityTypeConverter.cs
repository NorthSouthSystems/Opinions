namespace NorthSouthSystems;

public class IdentityTypeConverter : ITypeConverter
{
    public void Convert(ConvertTypeRequest request)
    {
        // All Nullable<T> instances box as their UnderlyingType.
        if (Throw.IfNull(request).Value?.GetType() == request.ConversionType.FlattenGenericNullable())
            request.Converted(request.Value);
    }
}