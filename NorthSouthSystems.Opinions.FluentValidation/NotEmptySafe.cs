namespace NorthSouthSystems.FluentValidation;

// https://docs.fluentvalidation.net/en/latest/built-in-validators.html#notempty-validator
// "Safe" is used to describe its behavior with "default valued" ValueType enumerables such as ImmutableArray.
public static class NotEmptySafeExtensions
{
    public static IRuleBuilderOptions<T, ImmutableArray<TProperty>> NotEmpty<T, TProperty>(
        this IRuleBuilder<T, ImmutableArray<TProperty>> rule) =>
        rule.Must(ia => !ia.IsDefaultOrEmpty);

    public static bool IsNotEmpty(object? value)
    {
        if (value is null)
            return false;

        if (value is string s)
            return !string.IsNullOrWhiteSpace(s);

        // Besides implementing the FluentValidation NotEmpty Validator "spec", this also protects us from
        // invoking methods on a "default valued" ValueType enumerable such as ImmutableArray (which throws an
        // InvalidOperationException or NullReferenceException when doing so).
        if (value.Equals(value.GetType().Default()))
            return false;

        if (value is ICollection c)
            return c.Count > 0;

        if (value is IEnumerable e)
            return e.Cast<object>().Any();

        return true;
    }
}