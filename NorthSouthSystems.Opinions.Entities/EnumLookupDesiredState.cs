using System.Reflection;

namespace NorthSouthSystems.Entities;

public static class EnumLookupDesiredState
{
    public static void Apply(IUnitOfWorkEntitySet<EnumLookup> enumLookups, params IEnumerable<Assembly> assemblies)
    {
        var current = EnumRegistry.FromEnumLookups(Throw.IfNull(enumLookups).Local);
        var future = EnumRegistry.FromAssemblies(assemblies);

        foreach ((string typeName, string name, long value) in current.DiffReturnAdditions(future))
            enumLookups.Add(new(typeName, name, value));
    }
}