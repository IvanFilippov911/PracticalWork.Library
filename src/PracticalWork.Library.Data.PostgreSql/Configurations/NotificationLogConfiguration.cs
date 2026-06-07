using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql.Configurations;

internal sealed class NotificationLogConfiguration : EntityConfigurationBase<NotificationLogEntity>
{
    public override void Configure(EntityTypeBuilder<NotificationLogEntity> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.NotificationType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.ReceiverEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(p => p.ErrorMessage)
            .HasMaxLength(1024);

        builder.Property(p => p.SentAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(p => new { p.NotificationType, p.BorrowId, p.SentAt });
        builder.HasIndex(p => p.ReaderId);
        builder.HasIndex(p => p.BookId);
    }
}
