using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticalWork.Library.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Readers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ArchiveLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ReaderId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookId = table.Column<Guid>(type: "uuid", nullable: true),
                    BorrowId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReceiverEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyReportMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BucketName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ObjectName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    PeriodFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodTo = table.Column<DateOnly>(type: "date", nullable: false),
                    NewBooksCount = table.Column<int>(type: "integer", nullable: false),
                    NewReadersCount = table.Column<int>(type: "integer", nullable: false),
                    BorrowedCount = table.Column<int>(type: "integer", nullable: false),
                    ReturnedCount = table.Column<int>(type: "integer", nullable: false),
                    OverdueCount = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyReportMetadata", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveLogs_BookId",
                table: "ArchiveLogs",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveLogs_JobRunId",
                table: "ArchiveLogs",
                column: "JobRunId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_BookId",
                table: "NotificationLogs",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_NotificationType_BorrowId_SentAt",
                table: "NotificationLogs",
                columns: new[] { "NotificationType", "BorrowId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_ReaderId",
                table: "NotificationLogs",
                column: "ReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyReportMetadata_IsDeleted_CreatedAt",
                table: "WeeklyReportMetadata",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyReportMetadata_ReportName",
                table: "WeeklyReportMetadata",
                column: "ReportName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchiveLogs");

            migrationBuilder.DropTable(
                name: "NotificationLogs");

            migrationBuilder.DropTable(
                name: "WeeklyReportMetadata");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Readers");
        }
    }
}
