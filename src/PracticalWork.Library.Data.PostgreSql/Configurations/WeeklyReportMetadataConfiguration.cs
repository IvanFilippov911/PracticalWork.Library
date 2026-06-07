using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql.Configurations;

internal sealed class WeeklyReportMetadataConfiguration : EntityConfigurationBase<WeeklyReportMetadataEntity>
{
    public override void Configure(EntityTypeBuilder<WeeklyReportMetadataEntity> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.ReportName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(p => p.BucketName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(p => p.ObjectName)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(p => p.ReportName)
            .IsUnique();

        builder.HasIndex(p => new { p.IsDeleted, p.CreatedAt });
    }
}
