using Microsoft.Extensions.Logging;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Jobs.Common;

/// <summary>
/// Общая политика выполнения джоб: таймаут + retry.
/// </summary>
public static class JobExecutionPolicy
{
    public static async Task ExecuteAsync(
        string jobName,
        JobConfiguration configuration,
        ILogger logger,
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        var maxRetries = Math.Max(0, configuration.MaxRetries);
        var timeoutMinutes = Math.Max(1, configuration.TimeoutMinutes);

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(timeoutMinutes));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCts.Token);

            try
            {
                await action(linkedCts.Token);
                return;
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                logger.LogError(
                    "{JobName}: превышен таймаут {TimeoutMinutes} минут (attempt {Attempt}/{MaxAttempts})",
                    jobName,
                    timeoutMinutes,
                    attempt + 1,
                    maxRetries + 1);

                if (attempt == maxRetries)
                {
                    throw;
                }
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "{JobName}: ошибка выполнения (attempt {Attempt}/{MaxAttempts})",
                    jobName,
                    attempt + 1,
                    maxRetries + 1);

                if (attempt == maxRetries)
                {
                    throw;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        }
    }
}
