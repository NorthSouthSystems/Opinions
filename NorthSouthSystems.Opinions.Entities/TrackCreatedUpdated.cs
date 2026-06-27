namespace NorthSouthSystems.Entities;

// ReSharper disable NotAccessedPositionalProperty.Global
public readonly record struct Created(TenantUserId By, DateTimeOffset Utc);
public readonly record struct Updated(TenantUserId By, DateTimeOffset Utc, int Version);
// ReSharper restore NotAccessedPositionalProperty.Global

public interface ITrackCreated
{
    Created Created { get; set; }
}

public interface ITrackUpdated
{
    Updated Updated { get; set; }
}