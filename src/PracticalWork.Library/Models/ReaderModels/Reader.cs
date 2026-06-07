using PracticalWork.Library.Models.BookModels;

namespace PracticalWork.Library.Models.ReaderModels;

/// <summary>
/// Модель, представляющая читателя библиотеки
/// </summary>
public sealed class Reader
{
    /// <summary>
    /// ФИО
    /// </summary>
    /// <remarks>Запись идет через пробел</remarks>
    public string FullName { get; set; }

    /// <summary>
    /// Номер телефона
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Email адрес
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Дата окончания действия карточки
    /// </summary>
    public DateOnly ExpiryDate { get; set; }

    /// <summary>
    /// Активность карточки
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Выданные книги
    /// </summary>
    public IReadOnlyList<Book> BorrowBooks { get; set; }
}
