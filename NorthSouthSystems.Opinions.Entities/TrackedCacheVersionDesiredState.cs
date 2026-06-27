using System.Reflection;

namespace NorthSouthSystems.Entities;

public static class TrackedCacheVersionDesiredState
{
    public static void Apply(IUnitOfWorkEntitySet<TrackedCacheVersion> trackedCacheVersions,
        params IEnumerable<Assembly> assemblies)
    {
        var trackedCacheVersionsByTypeName = Throw.IfNull(trackedCacheVersions)
            .Local
            .ToImmutableDictionary(tcv => tcv.TypeName, StringComparer.OrdinalIgnoreCase);

        var trackedCacheTypeNames = new TrackedCacheRegistry(assemblies)
            .All
            .Select(tc => tc.Name)
            .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (string addTrackedCacheTypeName in trackedCacheTypeNames
                     .Where(tctn => !trackedCacheVersionsByTypeName.ContainsKey(tctn)))
        {
            trackedCacheVersions.Add(new(addTrackedCacheTypeName));
        }

        foreach (var removeTrackedCacheVersion in trackedCacheVersionsByTypeName.Values
                     .Where(tcv => !trackedCacheTypeNames.Contains(tcv.TypeName)))
        {
            trackedCacheVersions.Remove(removeTrackedCacheVersion);
        }
    }
}