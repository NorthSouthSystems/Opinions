using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NorthSouthSystems.Entities;

/// <summary>
/// Intercepts SaveChanges[Async] to enforce that:
/// 1. Every changed Entity (except Owned Types) implements ISaveChangesPermissions.
/// 2. Modified entities AllowModify.
/// 3. Deleted entities AllowDelete.
/// </summary>
internal sealed class SaveChangesPermissionsInterceptor : SaveChangesInterceptor, IDryRunChangesInterceptor
{
    internal static SaveChangesPermissionsInterceptor Singleton { get; } = new();

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Validate(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Validate(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    void IDryRunChangesInterceptor.DryRun(DbContext? context) => Validate(context);

    private static void Validate(DbContext? context)
    {
        if (context is null)
            return;

        var changes = context.ChangeTracker
            .Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => !e.Metadata.IsOwned());

        var violations = changes.Select(change =>
            {
                if (change.Entity is not ISaveChangesPermissions permissions)
                    return string.Create(InvariantCulture, $"Entity '{change.Entity.GetType().FullName}' must implement {nameof(ISaveChangesPermissions)}.");

                if (change.State == EntityState.Modified && !permissions.AllowModify)
                    return string.Create(InvariantCulture, $"Entity '{change.Entity.GetType().FullName}' does not {nameof(ISaveChangesPermissions.AllowModify)}.");

                if (change.State == EntityState.Deleted && !permissions.AllowDelete)
                    return string.Create(InvariantCulture, $"Entity '{change.Entity.GetType().FullName}' does not {nameof(ISaveChangesPermissions.AllowDelete)}.");

                return null;
            })
            .Where(string.IsNotNullAndNotWhiteSpace);

        ArgumentExceptionX.ThrowIfAny(violations);
    }
}