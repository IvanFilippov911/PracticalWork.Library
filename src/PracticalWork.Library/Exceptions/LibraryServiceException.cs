namespace PracticalWork.Library.Exceptions;

/// <summary>
/// Исключение бизнес-логики сценариев библиотеки.
/// </summary>
public class LibraryServiceException: AppException
{
    /// <summary>
    /// Инициализирует исключение с сообщением об ошибке.
    /// </summary>
    /// <param name="message">Текст ошибки.</param>
    public LibraryServiceException(string message) : base(message)
    {
        
    }

    /// <summary>
    /// Инициализирует исключение с сообщением и внутренним исключением.
    /// </summary>
    /// <param name="message">Текст ошибки.</param>
    /// <param name="innerException">Внутреннее исключение.</param>
    public LibraryServiceException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
}
