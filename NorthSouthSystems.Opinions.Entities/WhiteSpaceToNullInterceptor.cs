using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NorthSouthSystems.Entities;

internal sealed class WhiteSpaceToNullInterceptor : SaveChangesInterceptor, IDryRunChangesInterceptor
{
    internal static WhiteSpaceToNullInterceptor Singleton { get; } = new();

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        WhiteSpaceToNull(eventData.Context);

        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        WhiteSpaceToNull(eventData.Context);

        return ValueTask.FromResult(result);
    }

    void IDryRunChangesInterceptor.DryRun(DbContext? context) => WhiteSpaceToNull(context);

    private static void WhiteSpaceToNull(DbContext? context)
    {
        if (context is null)
            return;

        var entities = context
            .ChangeTracker
            .Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        var properties = entities
            .SelectMany(e => e.Properties)
            .Where(p => p.Metadata.ClrType == typeof(string))
            .Where(p => !p.IsTemporary);

        foreach (var property in properties)
        {
            string? current = (string?)property.CurrentValue;

            if (current is null)
                continue;

            if (!string.IsNullOrWhiteSpace(current))
                continue;

            property.CurrentValue = null;

            if (property.EntityEntry.State is EntityState.Added)
                continue;

            property.IsModified = property.OriginalValue is not null;
        }
    }
}