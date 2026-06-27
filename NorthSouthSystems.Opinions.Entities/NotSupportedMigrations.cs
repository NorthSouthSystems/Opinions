using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace NorthSouthSystems.Entities;

internal sealed class NotSupportedMigrator : IMigrator
{
    public string GenerateScript(string? fromMigration = null, string? toMigration = null, MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default) =>
        throw new NotSupportedException();

    public bool HasPendingModelChanges() =>
        throw new NotSupportedException();

    public void Migrate(string? targetMigration = null) =>
        throw new NotSupportedException();

    public Task MigrateAsync(string? targetMigration = null, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}

internal sealed class NotSupportedRelationalDatabaseCreator : IRelationalDatabaseCreator
{
    public bool CanConnect() =>
        throw new NotSupportedException();

    public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public void Create() =>
        throw new NotSupportedException();

    public Task CreateAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public void CreateTables() =>
        throw new NotSupportedException();

    public Task CreateTablesAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public void Delete() =>
        throw new NotSupportedException();

    public Task DeleteAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public bool EnsureCreated() =>
        throw new NotSupportedException();

    public Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public bool EnsureDeleted() =>
        throw new NotSupportedException();

    public Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public bool Exists() =>
        throw new NotSupportedException();

    public Task<bool> ExistsAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public string GenerateCreateScript() =>
        throw new NotSupportedException();

    public bool HasTables() =>
        throw new NotSupportedException();

    public Task<bool> HasTablesAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}