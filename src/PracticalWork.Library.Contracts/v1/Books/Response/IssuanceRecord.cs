using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// Информация о факте выдачи книги.
/// </summary>
/// <param name="IssueStatus">Статус выдачи.</param>
/// <param name="DueDate">Плановая дата возврата.</param>
/// <param name="ReturnDate">Фактическая дата возврата.</param>
/// <param name="BorrowDate">Дата выдачи.</param>
public sealed record IssuanceRecord(
    BookIssueStatus IssueStatus,
    DateOnly DueDate,
    DateOnly ReturnDate,
    DateOnly BorrowDate
);
