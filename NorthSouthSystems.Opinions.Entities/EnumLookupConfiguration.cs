using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NorthSouthSystems.Entities;

public sealed class EnumLookupConfiguration() : ConventionEntityTypeConfiguration<EnumLookup>(Schemas.Shared)
{
    public override void Configure(EntityTypeBuilder<EnumLookup> builder)
    {
        base.Configure(Throw.IfNull(builder));

        builder.HasKey(x => new { x.TypeName, x.Name });
        builder.Property(x => x.TypeName).IsRequired();
        builder.Property(x => x.Name).IsRequired();
    }
}