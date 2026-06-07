using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Library.Contracts.v1.Reports.Request;
using PracticalWork.Library.Data.Reports.PostgreSql.Entities;
using PracticalWork.Library.Data.Reports.PostgreSql.Extensions;

namespace PracticalWork.Library.Data.Reports.PostgreSql.Repositories;

public class ActivityLogRepository: IActivityLogRepository
{
    private readonly ReportsDbContext _context;
    
    public ActivityLogRepository(ReportsDbContext context)
    {
        _context = context;
    }
    
    public async Task AddLogAsync(ActivityLog activityLog)
    {
        ActivityLogEntity entity = new()
        {
            EventDate = activityLog.EventDate,
            EventType = activityLog.EventType,
            Metadata = activityLog.SerializeEvent()
        };
        await _context.ActivityLogs.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<(IReadOnlyList<ActivityLog>, int)> GetLogsPageAsync(ActivityLogsPaginationRequest request)
    {
        var entities = await GetLogsAsync(
            request.EventDateFrom,
            request.EventDateTo,
            request.EventTypes,
            query => query.SkipTake(request));
        
        return (entities.Item1, entities.Item2);
    }


    public async Task<(IReadOnlyList<ActivityLog>, int)> GetLogsAsync(DateOnly? periodFrom, DateOnly? periodTo, string[]? eventTypes)
    {
        return await GetLogsAsync(periodFrom, periodTo, eventTypes, null);
    }
    
    private async Task<(IReadOnlyList<ActivityLog>, int)> GetLogsAsync(DateOnly? periodFrom, DateOnly? periodTo, string[]? eventTypes,
        Func<IQueryable<ActivityLogEntity>, IQueryable<ActivityLogEntity>>? query)
    {
        DateTime? dtFrom = periodFrom?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc); 
        DateTime? dtTo = periodTo?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var filteredEntities = _context.ActivityLogs
            .Where(l => eventTypes == null || eventTypes.Length == 0 || eventTypes.Contains(l.EventType))
            .Where(l => !dtFrom.HasValue || l.EventDate >= dtFrom)
            .Where(l => !dtTo.HasValue || l.EventDate <= dtTo);
        
        var count = await filteredEntities.CountAsync();
        
        if (query != null)
        {
            filteredEntities = query(filteredEntities);
        }
        
        var entities = await filteredEntities
            .Select(l => new {l.EventDate, l.EventType, l.Id, l.Metadata})
            .ToListAsync();
        
        return (entities
            .Select(e => new ActivityLog
            { 
                EventDate = e.EventDate, 
                EventType = e.EventType, 
                Event = ActivityLog.DeserializeEvent(e.EventType, e.Metadata)
            })
            .ToList(), count);
    }
}
