using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace NorthSouthSystems.FluentValidation;

public static class NotEmptyNumberOfExtensions
{
    // Common cases

    public static IRuleBuilderOptionsConditions<T, T> ExactlyOneOf<T>(this IRuleBuilder<T, T> rule,
        params Expression<Func<T, object?>>[] expressions) =>
        ExactlyNumberOf(rule, 1, expressions);

    public static IRuleBuilderOptionsConditions<T, T> AtLeastOneOf<T>(this IRuleBuilder<T, T> rule,
        params Expression<Func<T, object?>>[] expressions) =>
        AtLeastNumberOf(rule, 1, expressions);

    public static IRuleBuilderOptionsConditions<T, T> AtMostOneOf<T>(this IRuleBuilder<T, T> rule,
        params Expression<Func<T, object?>>[] expressions) =>
        AtMostNumberOf(rule, 1, expressions);

    // General cases

    public static IRuleBuilderOptionsConditions<T, T> ExactlyNumberOf<T>(this IRuleBuilder<T, T> rule,
        int number, params Expression<Func<T, object?>>[] expressions) =>
        NotEmptyNumberOf(rule, NotEmptyNumberOfMode.Exactly, number, expressions);

    public static IRuleBuilderOptionsConditions<T, T> AtLeastNumberOf<T>(this IRuleBuilder<T, T> rule,
        int number, params Expression<Func<T, object?>>[] expressions) =>
        NotEmptyNumberOf(rule, NotEmptyNumberOfMode.AtLeast, number, expressions);

    public static IRuleBuilderOptionsConditions<T, T> AtMostNumberOf<T>(this IRuleBuilder<T, T> rule,
        int number, params Expression<Func<T, object?>>[] expressions) =>
        NotEmptyNumberOf(rule, NotEmptyNumberOfMode.AtMost, number, expressions);

    // Implementation

    private enum NotEmptyNumberOfMode
    {
        Exactly,
        AtLeast,
        AtMost
    }

    private static IRuleBuilderOptionsConditions<T, T> NotEmptyNumberOf<T>(IRuleBuilder<T, T> rule,
        NotEmptyNumberOfMode mode, int number, params Expression<Func<T, object?>>[] expressions)
    {
        Throw.IfNull(rule);
        Throw.IfNegative(number);
        Throw.IfNull(expressions);
        Throw.IfZero(expressions.Length);

        // This only occurs when a Validator is constructed, which for Singletons is exactly once.
        // We take the precaution of using a FuncCache in case there are any Scoped or Transient
        // Validators that use these extensions.
        var funcs = expressions.Select(FuncCache.GetOrAdd).ToImmutableArray();
        string memberNamesCsv = string.Join(", ", funcs.Select(f => f.Key.MemberName));

        return rule.Custom((t, context) =>
        {
            int notEmptyCount = funcs.Select(f => f.Value(t)).Count(NotEmptySafeExtensions.IsNotEmpty);

            bool isValid = mode switch
            {
                NotEmptyNumberOfMode.Exactly => notEmptyCount == number,
                NotEmptyNumberOfMode.AtLeast => notEmptyCount >= number,
                NotEmptyNumberOfMode.AtMost => notEmptyCount <= number,

                _ => throw new NotSupportedException(mode.ToString())
            };

            if (isValid)
                return;

            string errorMessage = string.Create(InvariantCulture, $"{mode.ToString().SpaceCamelCase()} {number} of {memberNamesCsv} must be non-empty.");

            foreach (string memberName in funcs.Select(f => f.Key.MemberName))
                context.AddFailure(memberName, errorMessage);
        });
    }

    private static class FuncCache
    {
        internal record struct CacheKey(Type Type, string MemberName);
        private static readonly ConcurrentDictionary<CacheKey, Func<object, object?>> Cache = new();

        internal static KeyValuePair<CacheKey, Func<T, object?>> GetOrAdd<T>(Expression<Func<T, object?>> expression)
        {
            var key = new CacheKey(typeof(T), GetMemberName(expression));
            var func = Cache.GetOrAdd(key, _ => expression.Compile());

            return new(key, t => func(t!));
        }

        private static string GetMemberName<T>(Expression<Func<T, object?>> expression)
        {
            // Strip the boxing Convert if present.
            var body = expression.Body is UnaryExpression u && u.NodeType == ExpressionType.Convert
                ? u.Operand
                : expression.Body;

            return body is MemberExpression m
                ? m.Member.Name
                : expression.ToString();
        }
    }
}