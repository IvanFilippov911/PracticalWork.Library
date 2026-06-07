using PracticalWork.Library.Models.BookModels;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис сценария архивации старых книг.
/// </summary>
public interface IBookArchiveProcessingService
{
    /// <summary>
    /// Выполнить архивацию старых книг.
    /// </summary>
    /// <param name="thresholdDate">Пороговая дата последней выдачи.</param>
    /// <param name="maxBooks">Максимальное количество книг за запуск.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат архивации.</returns>
    Task<ArchiveBooksProcessingResult> ProcessAsync(
        DateOnly thresholdDate,
        int maxBooks,
        CancellationToken cancellationToken = default);
}
