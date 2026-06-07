using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Data.Reports.PostgreSql.Entities;

namespace PracticalWork.Library.Data.Reports.PostgreSql;

public class ReportsDbContext: DbContext
{
    private readonly TimeProvider _timeProvider;

    public ReportsDbContext(DbContextOptions<ReportsDbContext> options, TimeProvider timeProvider) : base(options)
    {
        _timeProvider = timeProvider;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ReportsDbContext).Assembly);
    }

    #region Set UpdateDate on SaveChanges

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetUpdateDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetUpdateDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetUpdateDates()
    {
        var updateDate = _timeProvider.GetUtcNow().UtcDateTime;

        var updatedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in updatedEntries)
        {
            if (entry.Entity is IEntity entity)
                entity.UpdatedAt = updateDate;
        }
    }

    #endregion

    /// <summary>
    /// Набор сущностей отчетов
    /// </summary>
    public DbSet<ReportEntity> Reports { get; set; }

    /// <summary>
    /// Набор сущностей журнала активности
    /// </summary>
    public DbSet<ActivityLogEntity> ActivityLogs { get; set; }
}
