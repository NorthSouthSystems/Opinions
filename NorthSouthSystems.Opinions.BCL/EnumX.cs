using NorthSouthSystems.Linq;

namespace NorthSouthSystems;

public static class EnumX
{
    public static IEnumerable<TEnum> GetValues<TEnum>()
        where TEnum : Enum =>
        Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

#pragma warning disable CA1034 // False positive.
    extension(Enum)
    {
        public static ImmutableArray<T> GetValuesExceptOne<T>(T except)
            where T : struct, Enum =>
            [.. Enum.GetValues<T>().ExceptOne(except)];
    }
#pragma warning restore
}