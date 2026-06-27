namespace NorthSouthSystems.Entities;

public interface ITrackCreatedUpdatedByProvider
{
    TenantUserId CurrentId { get; }
}