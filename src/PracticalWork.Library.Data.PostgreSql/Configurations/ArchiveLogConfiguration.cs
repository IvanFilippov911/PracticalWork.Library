using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql.Configurations;

internal sealed class ArchiveLogConfiguration : EntityConfigurationBase<ArchiveLogEntity>
{
    public override void Configure(EntityTypeBuilder<ArchiveLogEntity> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.BookTitle)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Reason)
            .HasMaxLength(1024);

        builder.Property(p => p.ProcessedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(p => p.JobRunId);
        builder.HasIndex(p => p.BookId);
    }
}
