namespace PracticalWork.Library.Models.NotificationModels;

/// <summary>
/// Кандидат на отправку напоминания о возврате книги.
/// </summary>
public sealed class ReturnReminderCandidate
{
    /// <summary>
    /// Идентификатор выдачи.
    /// </summary>
    public Guid BorrowId { get; init; }

    /// <summary>
    /// Идентификатор читателя.
    /// </summary>
    public Guid ReaderId { get; init; }

    /// <summary>
    /// Полное имя читателя.
    /// </summary>
    public required string ReaderFullName { get; init; }

    /// <summary>
    /// Email читателя.
    /// </summary>
    public required string ReaderEmail { get; init; }

    /// <summary>
    /// Идентификатор книги.
    /// </summary>
    public Guid BookId { get; init; }

    /// <summary>
    /// Название книги.
    /// </summary>
    public required string BookTitle { get; init; }

    /// <summary>
    /// Авторы книги.
    /// </summary>
    public required string[] BookAuthors { get; init; }

    /// <summary>
    /// Срок возврата книги.
    /// </summary>
    public DateOnly DueDate { get; init; }

    /// <summary>
    /// Было ли уже успешно отправлено напоминание в окне дедупликации.
    /// </summary>
    public bool HasRecentSuccessfulReminder { get; init; }
}
