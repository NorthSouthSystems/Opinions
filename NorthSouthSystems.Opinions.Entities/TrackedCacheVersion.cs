namespace NorthSouthSystems.Entities;

public sealed class TrackedCacheVersion : ISaveChangesPermissions, ITrackCreated, ITrackUpdated
{
    private TrackedCacheVersion() { }

    internal TrackedCacheVersion(string typeName) =>
        TypeName = Throw.IfNullOrWhiteSpace(typeName);

    public string TypeName { get; private init; } = null!;

    bool ISaveChangesPermissions.AllowModify => true;
    bool ISaveChangesPermissions.AllowDelete => true;

    public Created Created { get; private set; }
    Created ITrackCreated.Created { get => Created; set => Created = value; }

    public Updated Updated { get; private set; }
    Updated ITrackUpdated.Updated { get => Updated; set => Updated = value; }
}