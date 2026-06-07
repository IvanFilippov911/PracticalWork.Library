using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql.Configurations;

internal sealed class BookBorrowConfiguration : EntityConfigurationBase<BookBorrowEntity>
{
    public override void Configure(EntityTypeBuilder<BookBorrowEntity> builder)
    {
        builder.HasOne(b => b.Book)
            .WithMany(b => b.IssuanceRecords)
            .HasForeignKey(b => b.BookId);
    }
}