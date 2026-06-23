using System.IO.Compression;

namespace NorthSouthSystems.IO.Compression;

// Adapted from a conversation with ChatGPT on 2025-09-24.
/// <summary>
/// A ZipArchive consists only of ZipArchiveEntries; there is no hierarchy. This class offers a read-only
/// "directory" abstraction onto ZipArchive. Directories are inferred from slashes in ZipArchiveEntry.FullName.
/// </summary>
public sealed class ZipDirectory
{
    public static ZipDirectory Build(ZipArchive archive)
    {
        Throw.IfNull(archive);

        var rootDirectory = ConstructRoot();

        foreach (var entry in archive.Entries)
        {
            // Zips should exclusively use forward slashes; however, this defense is a trivial cost.
            string path = entry.FullName.Replace('\\', '/');

            if (string.IsNullOrEmpty(path))
                continue;

            var workingDirectory = rootDirectory;

            bool isEmptyDirectory = path.EndsWith('/');
            string[] pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            int childDirectoryCount = pathSegments.Length - (isEmptyDirectory ? 0 : 1);

            foreach (string childDirectoryName in pathSegments.Take(childDirectoryCount))
            {
                if (!workingDirectory._directoriesByName.TryGetValue(childDirectoryName, out var childDirectory))
                {
                    string childDirectoryPath = string.Concat(workingDirectory.Path, childDirectoryName, "/");
                    childDirectory = new(childDirectoryName, childDirectoryPath);

                    workingDirectory._directoriesByName.Add(childDirectoryName, childDirectory);
                }

                workingDirectory = childDirectory;
            }

            if (!isEmptyDirectory)
                workingDirectory._filesByName.Add(pathSegments[^1], entry);
        }

        return rootDirectory;
    }

    private ZipDirectory(string name, string path)
    {
        Name = name;
        Path = path;
    }

    public string Name { get; }

    /// <summary>Zip folder path with trailing forward slash for non-root and empty for root.</summary>
    public string Path { get; }

    private static ZipDirectory ConstructRoot() => new(string.Empty, string.Empty);
    public bool IsRoot => string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Path);

    public IReadOnlyDictionary<string, ZipDirectory> DirectoriesByName => _directoriesByName;
    public IReadOnlyDictionary<string, ZipArchiveEntry> FilesByName => _filesByName;

    private readonly Dictionary<string, ZipDirectory> _directoriesByName = [];
    private readonly Dictionary<string, ZipArchiveEntry> _filesByName = [];

    public IEnumerable<ZipArchiveEntry> FilesAllRecursively =>
        FilesByName.Values.Concat(
            DirectoriesByName.Values.SelectMany(d => d.FilesAllRecursively));
}