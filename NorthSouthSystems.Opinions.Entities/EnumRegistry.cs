using System.Diagnostics;
using System.Reflection;

namespace NorthSouthSystems.Entities;

public sealed class EnumRegistry
{
    public static EnumRegistry FromCsv(string csv) =>
        new(
            Throw.IfNullOrWhiteSpace(csv)
                .SplitQuotedRows(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180)
                .TakeColumnHeaders(CsvColumnNames, true)
                .GroupBy(row => (string)row["EnumTypeName"]!)
                .ToImmutableDictionary(
                    group => group.Key,
                    group => group.ToImmutableDictionary(
                        row => (string)row["Name"]!,
                        row => (long)row["Value"])));

    public string ToCsv() =>
        ValuesByNameByTypeName
            .OrderBy(kvp => kvp.Key)
            .SelectMany(envs =>
                envs.Value
                    .OrderBy(kvp => kvp.Value)
                    .Select(nv => new[] { envs.Key, nv.Key, nv.Value.ToString(InvariantCulture) }))
            .Prepend(CsvColumnNames)
            .JoinQuotedRows(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);

    private static readonly IReadOnlyCollection<string> CsvColumnNames = ["EnumTypeName", "Name", "Value"];

    public static EnumRegistry FromAssemblies(params IEnumerable<Assembly> assemblies)
    {
        var enums = (assemblies ?? [])
            .Distinct()
            .SelectMany(a => a.DefinedTypes)
            .Where(t => t.IsEnum)
            .Where(t => t.IsPublic)
            .Select(e => e.AsType())
            .ToImmutableArray();

        var enumsWithUnderlyingUlong = enums.Where(e => e.GetEnumUnderlyingType() == typeof(ulong));

        ArgumentExceptionX.ThrowIfAny(enumsWithUnderlyingUlong, "Enums with ulong UnderlyingType are not supported.");

        var enumsWithDuplicateNames = enums
            .CountBy(e => e.Name.ToUpperInvariant())
            .Where(c => c.Value > 1)
            .Select(c => c.Key);

        ArgumentExceptionX.ThrowIfAny(enumsWithDuplicateNames, "Enums must have case-insensitive unique names.");

        var valuesByNameByTypeName = enums.ToImmutableDictionary(
            e => e.Name,
            e =>
            {
                string[] names = e.GetEnumNames();

                var values = e.GetEnumValues()
                    .Cast<object>()
                    .Select(Convert.ToInt64)
                    .ToImmutableArray();

                if (names.Length != values.Length)
                    throw new UnreachableException();

                return names.Zip(values).ToImmutableDictionary(kv => kv.First, kv => kv.Second);
            });

        var enumsWithoutUndefined = valuesByNameByTypeName
            .Where(envs => !envs.Value.ContainsKey(EnumUndefinedName))
            .Select(envs => envs.Key);

        ArgumentExceptionX.ThrowIfAny(enumsWithoutUndefined, "Enums missing field named '_Undefined'.");

        var enumsUndefinedNotZero = valuesByNameByTypeName
            .Where(envs => envs.Value[EnumUndefinedName] != 0)
            .Select(envs => envs.Key);

        ArgumentExceptionX.ThrowIfAny(enumsUndefinedNotZero, "Enums '_Undefined' field must be 0.");

        var enumsWithDuplicateValues = valuesByNameByTypeName
            .Where(envs => envs.Value.CountBy(nv => nv.Value).Any(c => c.Value > 1))
            .Select(envs => envs.Key);

        ArgumentExceptionX.ThrowIfAny(enumsWithDuplicateValues, "Enums must not have any duplicate Values.");

        return new(valuesByNameByTypeName);
    }

    public const string EnumUndefinedName = "_Undefined";

    internal static EnumRegistry FromEnumLookups(IEnumerable<EnumLookup> enumLookups) =>
        new(
            Throw.IfNull(enumLookups)
                .GroupBy(el => el.TypeName)
                .ToImmutableDictionary(
                    group => group.Key,
                    group => group.ToImmutableDictionary(el => el.Name, el => el.Value)));

    private EnumRegistry(ImmutableDictionary<string, ImmutableDictionary<string, long>> valuesByNameByTypeName) =>
        ValuesByNameByTypeName = valuesByNameByTypeName;

    public ImmutableDictionary<string, ImmutableDictionary<string, long>> ValuesByNameByTypeName { get; }

    public void ThrowIfAnyBreakingChanges(EnumRegistry future)
    {
        Throw.IfNull(future);

        var errors = new List<string>();

        foreach (string typeName in ValuesByNameByTypeName.Keys)
        {
            if (!future.ValuesByNameByTypeName.ContainsKey(typeName))
            {
                errors.Add("Missing Enum Type: " + typeName);
                continue;
            }

            var currentNameValues = ValuesByNameByTypeName[typeName];
            var futureNameValues = future.ValuesByNameByTypeName[typeName];

            foreach (string name in currentNameValues.Keys)
            {
                if (!futureNameValues.TryGetValue(name, out long futureValue))
                    errors.Add(string.Create(InvariantCulture, $"Missing Enum Name: {typeName}.{name}"));
                else if (currentNameValues[name] != futureValue)
                    errors.Add(string.Create(InvariantCulture, $"Changed Enum Value: {typeName}.{name}"));
            }
        }

        ArgumentExceptionX.ThrowIfAny(errors);
    }

    public ImmutableArray<(string TypeName, string Name, long Value)> DiffReturnAdditions(EnumRegistry future)
    {
        Throw.IfNull(future);

        ThrowIfAnyBreakingChanges(future);

        var adds = new List<(string TypeName, string Name, long Value)>();

        foreach (string typeName in future.ValuesByNameByTypeName.Keys)
        {
            if (!ValuesByNameByTypeName.ContainsKey(typeName))
            {
                adds.AddRange(
                    future.ValuesByNameByTypeName[typeName]
                        .Select(nv => (typeName, nv.Key, nv.Value)));

                continue;
            }

            var currentNameValues = ValuesByNameByTypeName[typeName];
            var futureNameValues = future.ValuesByNameByTypeName[typeName];

            foreach (string name in futureNameValues.Keys)
            {
                if (!currentNameValues.ContainsKey(name))
                    adds.Add((typeName, name, futureNameValues[name]));
            }
        }

        return [.. adds.OrderBy(a => a.TypeName).ThenBy(a => a.Value)];
    }
}