using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.Models.ReportModels;

namespace PracticalWork.Library.MessageBroker.Handlers
{
    /// <summary>
    /// Сохраняет события библиотеки в журнал активности отчетного сервиса.
    /// </summary>
    public class LibraryEventHandler : IMessageHandler<BaseLibraryEvent>
    {
        private readonly IActivityLogRepository _activityLogRepository;

        /// <summary>
        /// Инициализирует обработчик событий библиотеки.
        /// </summary>
        /// <param name="activityLogRepository">Репозиторий журналов активности.</param>
        public LibraryEventHandler(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }

        /// <inheritdoc />
        public async Task HandleAsync(BaseLibraryEvent libraryEvent, CancellationToken cancellationToken)
        {
            var log = new ActivityLog
            {
                Event = libraryEvent,
                EventDate = libraryEvent.OccurredOn,
                EventType = libraryEvent.EventType
            };

            await _activityLogRepository.AddLogAsync(log);
        }
    }

}
