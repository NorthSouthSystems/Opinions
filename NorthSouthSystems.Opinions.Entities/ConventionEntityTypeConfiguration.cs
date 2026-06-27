using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NorthSouthSystems.Entities;

public abstract class ConventionEntityTypeConfiguration<TEntity>(string schema) : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        Throw.IfNull(builder);

        builder.ToTable(typeof(TEntity).Name, Throw.IfNullOrWhiteSpace(schema));

        ConfigureTrackCreatedUpdated(builder);
    }

    private static void ConfigureTrackCreatedUpdated(EntityTypeBuilder<TEntity> builder)
    {
        if (typeof(ITrackCreated).IsAssignableFrom(typeof(TEntity)))
        {
            builder.ComplexProperty<Created>(nameof(ITrackCreated.Created), cpb =>
            {
                var createdBy = cpb.Property<TenantUserId>(nameof(Created.By))
                    .HasColumnName("CreatedBy")
                    .IsRequired();

                createdBy.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
                createdBy.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

                var createdUtc = cpb.Property<DateTimeOffset>(nameof(Created.Utc))
                    .HasColumnName("CreatedUtc")
                    .IsRequired()
                    .HasDefaultValueSql("SYSUTCDATETIME()")
                    .ValueGeneratedOnAdd();

                createdUtc.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                createdUtc.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            });
        }

        if (typeof(ITrackUpdated).IsAssignableFrom(typeof(TEntity)))
        {
            builder.ComplexProperty<Updated>(nameof(ITrackUpdated.Updated), cpb =>
            {
                cpb.Property<TenantUserId>(nameof(Updated.By))
                    .HasColumnName("UpdatedBy")
                    .IsRequired();

                // Does not require Before or AfterSaveBehavior because the Interceptor will always
                // set it correctly.

                var updatedUtc = cpb.Property<DateTimeOffset>(nameof(Updated.Utc))
                    .HasColumnName("UpdatedUtc")
                    .IsRequired()
                    .HasDefaultValueSql("SYSUTCDATETIME()")
                    .ValueGeneratedOnAddOrUpdate();

                updatedUtc.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                updatedUtc.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                var updatedVersion = cpb.Property<int>(nameof(Updated.Version))
                    .HasColumnName("UpdatedVersion")
                    .IsRequired()
                    .HasDefaultValue(1)
                    .ValueGeneratedOnAddOrUpdate();

                updatedVersion.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                updatedVersion.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            // The name of the trigger is inconsequential; it signals how this Entity must be saved.
            // https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/breaking-changes?tabs=v7#sqlserver-tables-with-triggers
            builder.ToTable(tb => tb.HasTrigger("UpdatedUtcVersion"));
        }
    }
}