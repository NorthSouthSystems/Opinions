using System.Reflection;

namespace NorthSouthSystems.Entities;

public sealed class TrackedCacheRegistry(params IEnumerable<Assembly> assemblies)
{
    public ImmutableArray<Type> All { get; } =
    [
        .. (assemblies ?? [])
        .Distinct()
        .SelectMany(a => a.GetTypes())
        .Where(t => !t.IsAbstract && t.IsSubTypeOfGeneric(typeof(TrackedCache<,>)))
    ];
}