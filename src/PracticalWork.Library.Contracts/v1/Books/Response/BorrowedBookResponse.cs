using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// DTO ответа с информацией о выданной книге
/// </summary>
/// <param name="Title">Название книги</param>
/// <param name="Category">Категория книги</param>
/// <param name="Authors">Список авторов книги</param>
/// <param name="Description">Описание книги</param>
/// <param name="Year">Год издания книги</param>
/// <param name="IssueStatus">Статус выдачи книги</param>
/// <param name="DueDate">Дата планируемого возврата книги</param>
/// <param name="ReturnDate">Фактическая дата возврата книги</param>
/// <param name="BorrowDate">Дата выдачи книги</param>
public record BorrowedBookResponse(
    string Title,
    BookCategory Category,
    IReadOnlyList<string> Authors,
    string Description,
    int Year,
    BookIssueStatus IssueStatus,
    DateOnly DueDate,
    DateOnly ReturnDate,
    DateOnly BorrowDate
);