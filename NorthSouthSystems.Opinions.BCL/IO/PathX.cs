using System.IO;
using System.Runtime.CompilerServices;

namespace NorthSouthSystems.IO;

public static class PathX
{
    public static string GetDirectoryNameOfCallerFilePath([CallerFilePath] string? callerFilePath = null)
    {
        if (!Path.IsPathRooted(Throw.IfNull(callerFilePath)))
            throw new ArgumentOutOfRangeException(nameof(callerFilePath));

        string? directory = Path.GetDirectoryName(callerFilePath);

        if (string.IsNullOrEmpty(directory))
            throw new ArgumentOutOfRangeException(nameof(callerFilePath));

        return directory;
    }

    public static string GetFullPathRelativeToCallerFilePath(string relativePath, [CallerFilePath] string? callerFilePath = null)
    {
        if (Path.IsPathRooted(Throw.IfNull(relativePath)))
            throw new ArgumentOutOfRangeException(nameof(relativePath));

        // We use our helper method here because of its validations (DRY).
        string directory = GetDirectoryNameOfCallerFilePath(callerFilePath);

        return Path.Combine(directory, relativePath);
    }
}