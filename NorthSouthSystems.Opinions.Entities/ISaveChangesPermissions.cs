namespace NorthSouthSystems.Entities;

public interface ISaveChangesPermissions
{
    bool AllowModify { get; }
    bool AllowDelete { get; }
}