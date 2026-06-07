using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.BookModels;

namespace PracticalWork.Library.DTO.LibraryDtos;

/// <summary>
/// DTO для передачи информации о книге в библиотеке
/// </summary>
public class LibraryBookDto
{
    /// <summary>
    /// Название книги
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Авторы
    /// </summary>
    public IReadOnlyList<string> Authors { get; set; }
    
    /// <summary>
    /// Описание книги
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Год издания
    /// </summary>
    public int Year { get; set; }
    
    /// <summary>
    /// Категория книги
    /// </summary>
    public BookCategory Category { get; set; }
    
    /// <summary>
    /// Статус книги
    /// </summary>
    public BookStatus Status { get; set; }
    
    
    /// <summary>
    /// В архиве
    /// </summary>
    public bool IsArchived { get; set; }
    
    /// <summary>
    /// записи о выдаче
    /// </summary>
    public IReadOnlyList<IssuanceDto> IssuanceRecords { get; set; }
}