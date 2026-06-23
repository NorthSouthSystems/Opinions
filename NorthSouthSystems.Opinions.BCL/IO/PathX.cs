using System.IO;
using System.Runtime.CompilerServices;

namespace NorthSouthSystems.IO;

public static class PathX
{
    public static string? CrawlToSolutionDirectory(string? currentDirectoryOverride = null)
    {
        var directory = new DirectoryInfo(currentDirectoryOverride ?? Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            bool hasSolution = directory.GetFiles("*.sln?")
                .Select(f => f.Extension)
                .Any(e => e.Equals(".sln", StringComparison.OrdinalIgnoreCase)
                    || e.Equals(".slnx", StringComparison.OrdinalIgnoreCase));

            if (hasSolution)
                break;

            directory = directory.Parent;
        }

        return directory?.FullName;
    }

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