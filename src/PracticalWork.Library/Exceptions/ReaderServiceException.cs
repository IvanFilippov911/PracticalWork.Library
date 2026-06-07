namespace PracticalWork.Library.Exceptions;

/// <summary>
/// Исключение бизнес-логики сценариев работы с читателями.
/// </summary>
public class ReaderServiceException: AppException
{
    /// <summary>
    /// Инициализирует исключение с сообщением об ошибке.
    /// </summary>
    /// <param name="message">Текст ошибки.</param>
    public ReaderServiceException(string message) : base(message)
    {
    }

    /// <summary>
    /// Инициализирует исключение с сообщением и внутренним исключением.
    /// </summary>
    /// <param name="message">Текст ошибки.</param>
    /// <param name="innerException">Внутреннее исключение.</param>
    public ReaderServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
