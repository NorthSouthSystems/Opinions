using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NorthSouthSystems.Entities;

internal sealed class TrackCreatedUpdatedInterceptor(ITrackCreatedUpdatedByProvider byProvider)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Track(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Track(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Track(DbContext? context)
    {
        if (context is null)
            return;

        Throw.IfNull(byProvider);

        foreach (var entry in context.ChangeTracker.Entries())
        {
            bool added = entry.State == EntityState.Added;
            bool modified = entry.State == EntityState.Modified;

            if (added && entry.Entity is ITrackCreated tc)
                tc.Created = tc.Created with { By = byProvider.CurrentId };

            if ((added || modified) && entry.Entity is ITrackUpdated tu)
                tu.Updated = tu.Updated with { By = byProvider.CurrentId };
        }
    }
}