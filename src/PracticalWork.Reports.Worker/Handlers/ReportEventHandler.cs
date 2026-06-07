using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Reports.Worker.Abstractions;

namespace PracticalWork.Reports.Worker.Handlers;

/// <summary>
/// Обработчик события создания отчета.
/// </summary>
public sealed class ReportEventHandler : IMessageHandler<ReportCreateEvent>
{
    private readonly IReportGenerationService _reportGenerationService;

    /// <summary>
    /// Инициализирует обработчик события создания отчета.
    /// </summary>
    /// <param name="reportGenerationService">Сервис генерации отчета.</param>
    public ReportEventHandler(IReportGenerationService reportGenerationService)
    {
        _reportGenerationService = reportGenerationService;
    }

    /// <summary>
    /// Обрабатывает событие создания отчета и запускает его генерацию.
    /// </summary>
    /// <param name="reportEvent">Событие создания отчета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task HandleAsync(ReportCreateEvent reportEvent, CancellationToken cancellationToken)
    {
        await _reportGenerationService.GenerateReportAsync(
            reportEvent.Id,
            reportEvent.PeriodFrom,
            reportEvent.PeriodTo,
            reportEvent.EventTypes.ToArray(),
            cancellationToken);
    }
}
