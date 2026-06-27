using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NorthSouthSystems.Entities;

public sealed class TrackedCacheVersionConfiguration() : ConventionEntityTypeConfiguration<TrackedCacheVersion>(Schemas.Shared)
{
    public override void Configure(EntityTypeBuilder<TrackedCacheVersion> builder)
    {
        base.Configure(Throw.IfNull(builder));

        builder.HasKey(x => x.TypeName);
        builder.Property(x => x.TypeName).IsRequired();
    }
}