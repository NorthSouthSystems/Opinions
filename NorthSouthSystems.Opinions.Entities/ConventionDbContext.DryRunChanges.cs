using Microsoft.EntityFrameworkCore;

namespace NorthSouthSystems.Entities;

internal interface IDryRunChangesInterceptor
{
    void DryRun(DbContext? context);
}

public abstract partial class ConventionDbContext<TDbContext>
{
    public ImmutableArray<EntityChange> DryRunChanges()
    {
        ChangeTracker.DetectChanges();

        foreach (var interceptor in DryRunChangesInterceptors)
            interceptor.DryRun(this);

        ChangeTracker.DetectChanges();

        return
        [
            .. ChangeTracker
                .Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .GroupBy(e => e.Entity.GetType())
                .Select(eg => new EntityChange(
                    eg.Key.Name,
                    eg.Count(e => e.State == EntityState.Added),
                    eg.Count(e => e.State == EntityState.Modified),
                    eg.Count(e => e.State == EntityState.Deleted)))
        ];
    }
}