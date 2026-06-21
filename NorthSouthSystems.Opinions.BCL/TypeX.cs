using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NorthSouthSystems;

public static class TypeX
{
    public static object? Default(this Type type) =>
        Throw.IfNull(type).IsValueType
            ? RuntimeHelpers.GetUninitializedObject(type)
            : null;

    public static bool IsGenericNullable(this Type type) => Nullable.GetUnderlyingType(type) != null;
    public static Type FlattenGenericNullable(this Type type) => Nullable.GetUnderlyingType(type) ?? type;

    // Unfortunately, there is no simpler method to determine this. All Systems.Numerics interfaces
    // are recursive generics (i.e. IInterface<T> where T : IInterface<T>), so they can't be used
    // with "is" or "as" operators on instances or with Type.IsAssignable for Types (without Reflection).
    public static bool IsFloatingPoint(this Type type) =>
        FloatingPointTypes.Contains(Throw.IfNull(type));

    public static ImmutableHashSet<Type> FloatingPointTypes { get; } =
    [
        typeof(Half),
        typeof(float),
        typeof(double),
        typeof(decimal)
    ];

    // Unfortunately, there is no simpler method to determine this. All Systems.Numerics interfaces
    // are recursive generics (i.e. IInterface<T> where T : IInterface<T>), so they can't be used
    // with "is" or "as" operators on instances or with Type.IsAssignable for Types (without Reflection).
    public static bool IsIntegral(this Type type) =>
        IntegralTypes.Contains(Throw.IfNull(type));

    public static ImmutableHashSet<Type> IntegralTypes { get; } =
    [
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),

        typeof(Int128),
        typeof(BigInteger)
    ];

    internal static bool CanBeEnumUnderlyingType(this Type type) =>
        EnumUnderlyingTypes.Contains(Throw.IfNull(type));

    public static ImmutableHashSet<Type> EnumUnderlyingTypes { get; } =
    [
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong)
    ];

    public static ImmutableDictionary<Type, string> CSharpKeywordsByType { get; } =
        new Dictionary<Type, string>
            {
                [typeof(bool)] = "bool",
                [typeof(byte)] = "byte",
                [typeof(sbyte)] = "sbyte",
                [typeof(short)] = "short",
                [typeof(ushort)] = "ushort",
                [typeof(int)] = "int",
                [typeof(uint)] = "uint",
                [typeof(long)] = "long",
                [typeof(ulong)] = "ulong",
                [typeof(float)] = "float",
                [typeof(double)] = "double",
                [typeof(decimal)] = "decimal",

                [typeof(char)] = "char",
                [typeof(string)] = "string",

                [typeof(object)] = "object"
            }
            .ToImmutableDictionary();
}