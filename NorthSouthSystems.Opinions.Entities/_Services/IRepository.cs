namespace NorthSouthSystems.Entities;

public interface IReadOnlyRepository
{
    IReadOnlyEntitySet<EnumLookup> EnumLookups { get; }
    IReadOnlyEntitySet<TrackedCacheVersion> TrackedCacheVersions { get; }
}

public interface IAtomicCommandRepository : IReadOnlyRepository
{
    // We do NOT use IReadOnlyRepository because it is potentially served by a server replica.
    // We do NOT use IUnitOfWorkRepository because it is unnecessary for this read-only, non-transactional, and untracked query.
    // That leaves us with IAtomicCommandRepository.
    Task<DateTimeOffset> QuerySysDateTimeOffset(CancellationToken cancellationToken);
}

public interface IUnitOfWorkRepository : IReadOnlyRepository
{
    new IUnitOfWorkEntitySet<EnumLookup> EnumLookups { get; }
    new IUnitOfWorkEntitySet<TrackedCacheVersion> TrackedCacheVersions { get; }

    ImmutableArray<EntityChange> DryRunChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}