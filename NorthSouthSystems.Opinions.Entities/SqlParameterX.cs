using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.CompilerServices;

namespace NorthSouthSystems.Entities;

public static class SqlParameterX
{
    public static SqlParameter CreateIdOfT<T>(IEnumerable<T> ids,
        [CallerArgumentExpression(nameof(ids))] string? name = null) =>
        IdOfT.NewSqlParameter(CallerArgumentToParameterName(name), ids);

    public static SqlParameter CreateScalar(UInt128? value,
        ParameterDirection direction = ParameterDirection.Input,
        [CallerArgumentExpression(nameof(value))] string? name = null) =>
        direction == ParameterDirection.Input // Unfortunately, Throw.IfNotEqual does not allow enums at this time.
            ? CreateScalar(value?.ToBytesBigEndian(), direction, name)
            : throw new NotSupportedException(direction.ToString());

    // Using TValue value as the first parameter does not allow generic arguments to be inferred and causes callers that
    // do not explicitly pass generic arguments to use the CreateScaler(object value) overload instead.
    public static SqlParameter CreateScalar<TValueObject, TValue>(IStructValueObject<TValueObject, TValue>? value,
        ParameterDirection direction = ParameterDirection.Input,
        [CallerArgumentExpression(nameof(value))] string? name = null)
        where TValueObject : struct, IStructValueObject<TValueObject, TValue>
        where TValue : struct, IComparable<TValue> =>
        CreateScalar(value?.Value, direction, name);

    // Using TValue value as the first parameter does not allow generic arguments to be inferred and causes callers that
    // do not explicitly pass generic arguments to use the CreateScaler(object value) overload instead.
    public static SqlParameter CreateScalar<TValueObject, TValue>(IValueObject<TValueObject, TValue> value,
        ParameterDirection direction = ParameterDirection.Input,
        [CallerArgumentExpression(nameof(value))] string? name = null)
        where TValueObject : struct, IValueObject<TValueObject, TValue>
        where TValue : IComparable<TValue>
    {
        // The compiler doesn't understand that value must be a struct due to its recursive generic constraint, and it
        // complains about nullability with the value.Value invocation below. value cannot be used with Throw.IfNull
        // because its interfaces contain static abstracts without implementations, hence ArgumentNullException here.
        ArgumentNullException.ThrowIfNull(value);

        return CreateScalar(value.Value, direction, name);
    }

    public static SqlParameter CreateScalar(object? value,
        ParameterDirection direction = ParameterDirection.Input,
        [CallerArgumentExpression(nameof(value))] string? name = null) =>
        new(CallerArgumentToParameterName(name), value) { Direction = direction };

    private static string CallerArgumentToParameterName(string? name)
    {
        Throw.IfNullOrWhiteSpace(name);

        string[] parts = name.Split([' ', '.', '(', ')'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length > 1)
        {
            name = parts[^1];

            if (name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                name = parts[^2] + name;
        }

        return string.Create(InvariantCulture, $"@{char.ToUpper(name[0], InvariantCulture)}{(name.Length > 1 ? name[1..] : string.Empty)}");
    }
}