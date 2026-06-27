using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NorthSouthSystems.Entities;

internal sealed class ReadOnlyInterceptor : SaveChangesInterceptor
{
    internal static ReadOnlyInterceptor Singleton { get; } = new();

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) =>
        throw new NotSupportedException();

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}