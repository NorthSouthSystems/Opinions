using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NorthSouthSystems.Entities;

public abstract class ConventionInfoTypeConfiguration<TInfo>(string schema) : IEntityTypeConfiguration<TInfo>
    where TInfo : class
{
    public virtual void Configure(EntityTypeBuilder<TInfo> builder)
    {
        Throw.IfNull(builder);

        builder.HasNoKey();

        builder.ToView(
            typeof(TInfo).Name.RemoveSuffix("Info", StringComparison.OrdinalIgnoreCase),
            Throw.IfNullOrWhiteSpace(schema));
    }
}