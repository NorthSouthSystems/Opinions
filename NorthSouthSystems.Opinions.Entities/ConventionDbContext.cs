using Microsoft.EntityFrameworkCore;

namespace NorthSouthSystems.Entities;

public abstract class ConventionDbContext(DbContextOptions options)
    : DbContext(options)
{
    internal static ImmutableArray<IDryRunChangesInterceptor> DryRunChangesInterceptors { get; } =
    [
        WhiteSpaceToNullInterceptor.Singleton,
        // Unnecessary for DryRun, and the dependency chain would be non-trivial.
        // sp.GetRequiredService<TrackCreatedUpdatedInterceptor>(),
        SaveChangesPermissionsInterceptor.Singleton
    ];
}