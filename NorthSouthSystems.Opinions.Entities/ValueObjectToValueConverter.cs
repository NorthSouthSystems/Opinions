using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NorthSouthSystems.Entities;

internal sealed class ValueObjectToValueConverter<TValueObject, TValue>()
    : ValueConverter<TValueObject, TValue>(id => id.Value, value => ConstructNoValidation(value))
    where TValueObject : struct, IValueObject<TValueObject, TValue>
    where TValue : IComparable<TValue>
{
    private static TValueObject ConstructNoValidation(TValue value) =>
        ValueObjectFactory<TValueObject, TValue>.ConstructNoValidation(value);
}