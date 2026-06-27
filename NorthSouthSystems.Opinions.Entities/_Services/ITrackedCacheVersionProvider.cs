namespace NorthSouthSystems.Entities;

public interface ITrackedCacheVersionProvider
{
    int GetVersion(Type trackedCacheType);
}