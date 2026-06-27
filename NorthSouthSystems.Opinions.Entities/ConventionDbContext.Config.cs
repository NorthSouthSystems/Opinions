using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace NorthSouthSystems.Entities;

public abstract partial class ConventionDbContext<TDbContext> : ConventionDbContext, IAtomicCommandRepository, IUnitOfWorkRepository
    where TDbContext : ConventionDbContext<TDbContext>
{
    protected ConventionDbContext(DbContextOptions<TDbContext> options, ITrackCreatedUpdatedByProvider trackCreatedUpdatedByProvider)
        : base(options)
    {
        TrackCreatedUpdatedByProvider = Throw.IfNull(trackCreatedUpdatedByProvider);

        _enumLookups = new(this);
        _trackedCacheVersions = new(this);
    }

    public ITrackCreatedUpdatedByProvider TrackCreatedUpdatedByProvider { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Throw.IfNull(modelBuilder);

        // A no-op as of Microsoft.EntityFrameworkCore version 9.0.9.
        base.OnModelCreating(modelBuilder);

        ApplyConfigurations(modelBuilder);
        ConfigureForeignKeys(modelBuilder);
    }

    private static void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), TypeFilter);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TDbContext).Assembly, TypeFilter);
        return;

        bool TypeFilter(Type type) => !type.IsAbstract && type.IsSubTypeOfGeneric(typeof(IEntityTypeConfiguration<>));
    }

    private static void ConfigureForeignKeys(ModelBuilder modelBuilder)
    {
        foreach (var foreignKey in modelBuilder.Model
                     .GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Throw.IfNull(optionsBuilder);

        base.OnConfiguring(optionsBuilder);

        // EF Migrations must be explicitly invoked to run; however, because of the catastrophic effects
        // that could occur if that were to ever happen accidentally, we are also explicitly blocking them.
        optionsBuilder.ReplaceService<IMigrator, NotSupportedMigrator>();
        optionsBuilder.ReplaceService<IRelationalDatabaseCreator, NotSupportedRelationalDatabaseCreator>();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        Throw.IfNull(configurationBuilder);

        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<string>().AreUnicode(false);
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetUtcDateTimeConverter>();

        configurationBuilder.Properties<UInt128>()
            .HaveConversion<UInt128ToBinary16Converter>()
            .HaveColumnType("BINARY(16)")
            .HaveMaxLength(16)
            .AreFixedLength();

        ConfigureValueObjectProperties(configurationBuilder);
    }

    protected abstract IEnumerable<Assembly> GetValueObjectAssemblies();

    private void ConfigureValueObjectProperties(ModelConfigurationBuilder configurationBuilder)
    {
        var valueObjectTypes = GetValueObjectAssemblies()
            .Distinct()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && t.IsSubTypeOfGeneric(typeof(IValueObject<,>)))
            .ToImmutableArray();

        foreach (var valueObjectType in valueObjectTypes)
        {
            var genericArgs = valueObjectType.GetSubTypeOfGeneric(typeof(IValueObject<,>))!.GetGenericArguments();

            ConfigureValueObjectPropertiesOfTypeMethod.MakeGenericMethod(genericArgs)
                .Invoke(null, [configurationBuilder]);
        }
    }

    private static readonly MethodInfo ConfigureValueObjectPropertiesOfTypeMethod = typeof(ConventionDbContext<TDbContext>)
        .GetMethod(nameof(ConfigureValueObjectPropertiesOfType), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static void ConfigureValueObjectPropertiesOfType<TValueObject, TValue>(ModelConfigurationBuilder builder)
        where TValueObject : struct, IValueObject<TValueObject, TValue>
        where TValue : IComparable<TValue>
    {
        builder.Properties<TValueObject>()
            .HaveConversion<ValueObjectToValueConverter<TValueObject, TValue>>()
            .HaveSentinel(ValueObjectFactory<TValueObject, TValue>.ConstructNoValidation(default!));
    }
}