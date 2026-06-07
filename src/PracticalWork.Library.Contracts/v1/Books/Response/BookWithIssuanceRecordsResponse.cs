using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// Ответ с деталями книги, включая историю выдач
/// </summary>
/// <param name="Title">Название книги</param>
/// <param name="Category">Категория книги</param>
/// <param name="Authors">Список авторов книги</param>
/// <param name="Description">Описание книги</param>
/// <param name="Year">Год издания книги</param>
/// <param name="Status">Текущий статус книги</param>
/// <param name="IsArchived">Признак архивированности книги</param>
/// <param name="Records">Список записей о выдаче книги</param>
public sealed record BookWithIssuanceRecordsResponse(
    string Title,
    BookCategory Category,
    IReadOnlyList<string> Authors,
    string Description,
    int Year,
    BookStatus Status,
    bool IsArchived,
    IReadOnlyList<IssuanceRecord> Records
) : AbstractBook(Title, Authors, Description, Year);
