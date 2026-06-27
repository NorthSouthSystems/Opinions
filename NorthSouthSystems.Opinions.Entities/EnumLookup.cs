namespace NorthSouthSystems.Entities;

public sealed class EnumLookup : ISaveChangesPermissions, ITrackCreated
{
    internal EnumLookup(string typeName, string name, long value)
    {
        TypeName = Throw.IfNullOrWhiteSpace(typeName);
        Name = Throw.IfNullOrWhiteSpace(name);
        Value = value;
    }

    public string TypeName { get; private init; }
    public string Name { get; private init; }
    public long Value { get; private init; }

    bool ISaveChangesPermissions.AllowModify => false;
    bool ISaveChangesPermissions.AllowDelete => false;

    public Created Created { get; private set; }
    Created ITrackCreated.Created { get => Created; set => Created = value; }
}