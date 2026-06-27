using Microsoft.EntityFrameworkCore;

namespace NorthSouthSystems.Entities;

public abstract partial class ConventionDbContext<TDbContext>
{
    public Task<DateTimeOffset> QuerySysDateTimeOffset(CancellationToken cancellationToken) =>
        Database.SqlQueryRaw<DateTimeOffset>("SELECT SYSDATETIMEOFFSET() AS [Value]") // EF Core requires [Value].
            .SingleAsync(cancellationToken);

    public DbSet<EnumLookup> EnumLookups => Set<EnumLookup>();
    IReadOnlyEntitySet<EnumLookup> IReadOnlyRepository.EnumLookups => _enumLookups;
    IUnitOfWorkEntitySet<EnumLookup> IUnitOfWorkRepository.EnumLookups => _enumLookups;
    private readonly DbEntitySet<EnumLookup> _enumLookups;

    public DbSet<TrackedCacheVersion> TrackedCacheVersions => Set<TrackedCacheVersion>();
    IReadOnlyEntitySet<TrackedCacheVersion> IReadOnlyRepository.TrackedCacheVersions => _trackedCacheVersions;
    IUnitOfWorkEntitySet<TrackedCacheVersion> IUnitOfWorkRepository.TrackedCacheVersions => _trackedCacheVersions;
    private readonly DbEntitySet<TrackedCacheVersion> _trackedCacheVersions;
}