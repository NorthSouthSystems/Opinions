using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace NorthSouthSystems;

public static class ArgumentExceptionX
{
    public static void ThrowIfAny<T>(IEnumerable<T>? enumerable,
        string? messagePrefix = null, bool messageIncludeIndices = false,
        string? originalParamName = null, [CallerArgumentExpression(nameof(enumerable))] string? paramName = null)
    {
        if (enumerable is null)
            return;

        StringBuilder? message = null;
        int index = 0;

        foreach (var t in enumerable)
        {
            if (message is null)
            {
                message = new(messagePrefix);

                if (!string.IsNullOrEmpty(messagePrefix) && !messagePrefix.EndsWith('\n'))
                    message.AppendLine();
            }
            else
                message.AppendLine();

            if (messageIncludeIndices)
            {
                message.Append(index++.ToString(InvariantCulture));
                message.Append(": ");
            }

            message.Append(t?.ToString());
        }

        if (message is null)
            return;

        throw new ArgumentException(message.ToString(), originalParamName ?? paramName);
    }

    public static void ThrowIfDefault<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : struct
    {
        if (argument is null)
            throw new ArgumentNullException(paramName, "Value cannot be null.");

        ThrowIfDefault(argument.Value, paramName);
    }

    public static void ThrowIfDefault<T>(T argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : struct
    {
        if (argument.Equals(default(T)))
            throw new ArgumentException("Value cannot be default.", paramName);
    }
}