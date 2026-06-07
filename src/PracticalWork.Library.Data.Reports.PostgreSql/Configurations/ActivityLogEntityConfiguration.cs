using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Library.Data.Reports.PostgreSql.Entities;

namespace PracticalWork.Library.Data.Reports.PostgreSql.Configurations;

public class ActivityLogEntityConfiguration: EntityConfigurationBase<ActivityLogEntity>
{
    public override void Configure(EntityTypeBuilder<ActivityLogEntity> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.EventType)
            .IsRequired();
        builder.Property(e => e.Metadata)
            .HasColumnType("jsonb");
        
    }
}