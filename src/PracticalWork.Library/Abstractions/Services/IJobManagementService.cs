using PracticalWork.Library.Models.JobModels;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Управление фоновыми задачами библиотеки.
/// </summary>
public interface IJobManagementService
{
    /// <summary>
    /// Получить список настроенных задач.
    /// </summary>
    IReadOnlyList<ScheduledJobInfo> GetJobs();

    /// <summary>
    /// Запустить задачу вручную.
    /// </summary>
    Task TriggerAsync(string jobName, CancellationToken cancellationToken = default);
}
