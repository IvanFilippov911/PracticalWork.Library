namespace PracticalWork.Library.Exceptions;

/// <summary>
/// Исключение, возникающее при ошибках работы с сервисом книг
/// </summary>
public sealed class BookServiceException : AppException
{
    public BookServiceException(string message) : base($"{message}")
    {
    }

    public BookServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}