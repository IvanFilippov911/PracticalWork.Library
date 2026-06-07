using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql.Configurations;

internal sealed class ArchiveJobRunConfiguration : EntityConfigurationBase<ArchiveJobRunEntity>
{
    public override void Configure(EntityTypeBuilder<ArchiveJobRunEntity> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.StartedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(p => p.CompletedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(p => p.StartedAt);
    }
}
